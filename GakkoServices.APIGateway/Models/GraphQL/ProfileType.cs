using System;
using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using RawRabbit;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class ProfileType : ObjectGraphType<Profile>
    {
        public ProfileType(ContextServiceLocator ContextServiceLocator, IBusClient queue)
        {
            Name = "Profile";

            Field<StringGraphType>("id", resolve: context => context.Source.Id.ToString());
            Field(x => x.Username);
            Field(x => x.Level);
            Field(x => x.TrainerCode);
            Field(x => x.Gender);
            Field<PokemonType>(
                "featuredPokemon",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "n" }),
                resolve: context => {
                    var responseTask = queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                        new PokemonRequestMessage {
                            Id = context.Source.FeaturedPokemon[context.GetArgument<int>("n")],
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
            Field<ListGraphType<PokemonType>>(
                "featuredPokemen",
                resolve: context => {
                    var responseTasks = new List<Task<ResultMessage>>();
                    foreach (Guid pokemon in context.Source.FeaturedPokemon) {
                        responseTasks.Add(
                            queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                                new PokemonRequestMessage {
                                    Id = pokemon,
                                }
                            )
                        );
                    }
                    var pokemen = new List<Pokemon>();
                    foreach (var result in Task.WhenAll(responseTasks).Result) {
                        var data = result.data as PokemonData;
                        pokemen.Add(
                            new Pokemon {
                                Id = data.Id,
                                Name = data.Name,
                                PokedexNumber = data.PokedexNumber,
                                ImageUrl = data.ImageUrl,
                            }
                        );
                    }
                    return pokemen;
                }
            );
            Field<StringGraphType>("teamId", resolve: context => context.Source.TeamId.ToString());
            Field<TeamType>(
                "team",
                resolve: context => {
                    var responseTask = queue.RequestAsync<TeamRequestMessage, ResultMessage>(
                        new TeamRequestMessage {
                            Id = context.Source.TeamId,
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
        }
    }
}
