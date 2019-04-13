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
            FieldAsync<PokemonType>(
                "featuredPokemon",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "n" }),
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                        new PokemonRequestMessage {
                            Id = context.Source.FeaturedPokemon[context.GetArgument<int>("n")],
                        }
                    );
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
            FieldAsync<ListGraphType<PokemonType>>(
                "featuredPokemen",
                resolve: async context => {
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
                    foreach (var result in await Task.WhenAll(responseTasks)) {
                        var data = result.data as PokemonData;
                        pokemen.Add(
                            new Pokemon {
                                Id = data.Id,
                                Name = data.Name,
                                PokedexNumber = data.PokedexNumber,
                                SpriteImageUrl = data.SpriteImageUrl,
                                PogoImageUrl = data.PogoImageUrl,
                            }
                        );
                    }
                    return pokemen;
                }
            );
            FieldAsync<BadgeType>(
                "featuredBadge",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "n" }),
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<BadgeRequestMessage, ResultMessage>(
                        new BadgeRequestMessage {
                            Id = context.Source.FeaturedBadges[context.GetArgument<int>("n")],
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
                "featuredBadges",
                resolve: async context => {
                    var responseTasks = new List<Task<ResultMessage>>();
                    foreach (Guid badge in context.Source.FeaturedBadges) {
                        responseTasks.Add(
                            queue.RequestAsync<BadgeRequestMessage, ResultMessage>(
                                new BadgeRequestMessage {
                                    Id = context.Source.FeaturedBadges[context.GetArgument<int>("n")],
                                }
                            )
                        );
                    }
                    var badges = new List<Badge>();
                    foreach (var result in await Task.WhenAll(responseTasks)) {
                        var badgeData = result.data as BadgeData;
                        badges.Add(
                            new Badge {
                                Id = badgeData.Id,
                                Name = badgeData.Name,
                                ImageUrl = badgeData.ImageUrl,
                        });
                    }
                    return badges;
                }
            );
            Field<StringGraphType>("teamId", resolve: context => context.Source.TeamId.ToString());
            FieldAsync<TeamType>(
                "team",
                resolve: async context => {
                    var responseTask = await queue.RequestAsync<TeamRequestMessage, ResultMessage>(
                        new TeamRequestMessage {
                            Id = context.Source.TeamId,
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
        }
    }
}
