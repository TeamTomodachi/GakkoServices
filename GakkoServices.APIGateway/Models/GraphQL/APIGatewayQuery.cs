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
        public APIGatewayQuery(IBusClient queue)
        {
            Field<ProfileType>(
                "profile",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context => {
                    var responseTask = queue.RequestAsync<ProfileRequestMessage, ResultMessage>(
                        new ProfileRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var profileData = responseTask.Result.data as ProfileData;
                    return new Profile {
                        Id = profileData.Id,
                        Username = profileData.Username,
                        TrainerCode = profileData.TrainerCode,
                        Level = profileData.Level,
                        TeamId = profileData.TeamId,
                        FeaturedPokemon1 = profileData.FeaturedPokemon1,
                        FeaturedPokemon2 = profileData.FeaturedPokemon2,
                        FeaturedPokemon3 = profileData.FeaturedPokemon3,
                    };
                }
            );

            Field<PokemonType>(
                "pokemon",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context => {
                    var responseTask = queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                        new PokemonRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var pokemonData = responseTask.Result.data as PokemonData;
                    return new Pokemon {
                        Id = pokemonData.Id,
                        Name = pokemonData.Name,
                        PokedexNumber = pokemonData.PokedexNumber,
                        ImageUrl = pokemonData.ImageUrl,
                    };
                }
            );

            Field<TeamType>(
                "team",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context => {
                    var responseTask = queue.RequestAsync<TeamRequestMessage, ResultMessage>(
                        new TeamRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var teamData = responseTask.Result.data as TeamData;
                    return new Team {
                        Id = teamData.Id,
                        Name = teamData.Name,
                        Color = teamData.Color,
                        ImageUrl = teamData.ImageUrl,
                    };
                }
            );

            Field<ListGraphType<TeamType>>(
                "teams",
                resolve: context => {
                    var responseTask = queue.RequestAsync<TeamsRequestMessage, ResultMessage>(
                        new TeamsRequestMessage()
                    );
                    var teamDatas = responseTask.Result.data as List<TeamData>;
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
