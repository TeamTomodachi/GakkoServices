using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
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
            Field<PokemonType>(
                "featuredPokemon1",
                resolve: context => {
                    var responseTask = queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                        new PokemonRequestMessage {
                            Id = context.Source.FeaturedPokemon1,
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
            Field<PokemonType>(
                "featuredPokemon2",
                resolve: context => {
                    var responseTask = queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                        new PokemonRequestMessage {
                            Id = context.Source.FeaturedPokemon2,
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
            Field<PokemonType>(
                "featuredPokemon3",
                resolve: context => {
                    var responseTask = queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                        new PokemonRequestMessage {
                            Id = context.Source.FeaturedPokemon3,
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
