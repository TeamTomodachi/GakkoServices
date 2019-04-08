using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using RawRabbit;
using Newtonsoft.Json;

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
                    var thing = await helpers.GetProfileFromAccountId(userId);
                    return thing;
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
                    Profile p = new Profile();
                    p.Id = profileData.Id;
                    p.Username = profileData.Username;
                    p.TrainerCode = profileData.TrainerCode;
                    p.Level = profileData.Level;
                    p.TeamId = profileData.TeamId;
                    p.FeaturedPokemon = new List<Guid>();
                    p.FeaturedPokemon.Add(profileData.FeaturedPokemon1);
                    p.FeaturedPokemon.Add(profileData.FeaturedPokemon2);
                    p.FeaturedPokemon.Add(profileData.FeaturedPokemon3);
                    return p;
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

            FieldAsync<BadgeType>(
                "badge",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<BadgeRequestMessage, ResultMessage>(
                        new BadgeRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var badgeData = responseTask.data as BadgeData;
                    return new Badge {
                        Id = badgeData.Id,
                        Name = badgeData.Name,
                        ImageUrl = badgeData.ImageUrl,
                    };
                }
            );

            FieldAsync<ListGraphType<BadgeType>>(
                "badges",
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<BadgesRequestMessage, ResultMessage>(
                        new BadgesRequestMessage()
                    );
                    var badgeDatas = responseTask.data as List<BadgeData>;
                    var badges = new List<Badge>();
                    foreach (var badgeData in badgeDatas) {
                        badges.Add(new Badge {
                            Id = badgeData.Id,
                            Name = badgeData.Name,
                            ImageUrl = badgeData.ImageUrl,
                        });
                    }
                    return badges;
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

            FieldAsync<ListGraphType<PokemonType>>(
                "pokemen",
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<PokemenRequestMessage, ResultMessage>(
                        new PokemenRequestMessage()
                    );
                    var pokemanDatas = responseTask.data as List<PokemonData>;
                    var pokemans = new List<Pokemon>();
                    foreach (var pokemonData in pokemanDatas) {
                        pokemans.Add(new Pokemon {
                            Id = pokemonData.Id,
                            Name = pokemonData.Name,
                            PokedexNumber = pokemonData.PokedexNumber,
                            SpriteImageUrl = pokemonData.SpriteImageUrl,
                            PogoImageUrl = pokemonData.PogoImageUrl,
                        });
                    }
                    return pokemans;
                }
            );
        }
    }
}
