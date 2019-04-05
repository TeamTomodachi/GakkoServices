using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using RawRabbit;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class APIGatewayQuery : ObjectGraphType
    {
        public APIGatewayQuery(IBusClient queue, QueueHelpers helpers)
        {
            FieldAsync<ProfileType>(
                "me",
                resolve: async context =>
                {
                    var userId = (await helpers.AuthenticateFromContext(context)).UserId;
                    return helpers.GetProfileFromAccountId(userId);
                }
            );
            FieldAsync<ProfileType>(
                "profile",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<ProfileRequestMessage, ResultMessage>(
                        new ProfileRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var profileData = responseTask.data as ProfileData;
                    return new Profile {
                        Id = profileData.Id,
                        Username = profileData.Username,
                        TrainerCode = profileData.TrainerCode,
                        Level = profileData.Level,
                        TeamId = profileData.TeamId,
                        FeaturedPokemon = {
                            profileData.FeaturedPokemon1,
                            profileData.FeaturedPokemon2,
                            profileData.FeaturedPokemon3,
                        },
                    };
                }
            );

            FieldAsync<PokemonType>(
                "pokemon",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "name" },
                    new QueryArgument<IntGraphType> { Name = "pokedexNumber" }
                ),
                resolve: async context => {
                    PokemonRequestMessage message;
                    if (context.HasArgument("id"))
                    {
                        message = new PokemonRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        };
                    }
                    else if (context.HasArgument("name"))
                    {
                        message = new PokemonRequestMessage {
                            Name = context.GetArgument<string>("name"),
                        };
                    }
                    else if (context.HasArgument("pokedexNumber"))
                    {
                        message = new PokemonRequestMessage {
                            PokedexNumber = context.GetArgument<int>("pokedexNumber"),
                        };
                    }
                    else {
                        throw new ArgumentNullException();
                    }
                    var responseTask = await queue.RequestAsync<PokemonRequestMessage, ResultMessage>(message);
                    var pokemonData = responseTask.data as PokemonData;
                    return new Pokemon {
                        Id = pokemonData.Id,
                        Name = pokemonData.Name,
                        PokedexNumber = pokemonData.PokedexNumber,
                        SpriteImageUrl = pokemonData.SpriteImageUrl,
                        PogoImageUrl = pokemonData.PogoImageUrl,
                    };
                }
            );

            FieldAsync<TeamType>(
                "team",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<TeamRequestMessage, ResultMessage>(
                        new TeamRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var teamData = responseTask.data as TeamData;
                    return new Team {
                        Id = teamData.Id,
                        Name = teamData.Name,
                        Color = teamData.Color,
                        ImageUrl = teamData.ImageUrl,
                    };
                }
            );

            FieldAsync<ListGraphType<TeamType>>(
                "teams",
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<TeamsRequestMessage, ResultMessage>(
                        new TeamsRequestMessage()
                    );
                    var teamDatas = responseTask.data as List<TeamData>;
                    var teams = new List<Team>();
                    foreach (var teamData in teamDatas) {
                        teams.Add(new Team {
                            Id = teamData.Id,
                            Name = teamData.Name,
                            Color = teamData.Color,
                            ImageUrl = teamData.ImageUrl,
                        });
                    }
                    return teams;
                }
            );
        }
    }
}
