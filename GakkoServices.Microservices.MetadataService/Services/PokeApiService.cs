﻿using GakkoServices.Microservices.MetadataService.Models;
using PokeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.MetadataService.Services
{
    public class PokeApiService
    {
        public async Task<PokemonSpecies> GetPokemonSpeciesAsync(int pokedexId)
        {
            var p = await DataFetcher.GetApiObject<PokemonSpecies>(pokedexId);
            return p;
        }
        public async Task<PokemonSpecies> GetPokemonSpeciesAsync(string pokemonName)
        {
            var p = await DataFetcher.GetNamedApiObject<PokemonSpecies>(pokemonName);
            return p;
        }
        public async Task<Pokemon> GetPokemonAsync(int pokedexId)
        {
            var p = await DataFetcher.GetApiObject<Pokemon>(pokedexId);
            return p;
        }
        public async Task<Pokemon> GetPokemonAsync(string pokemonName)
        {
            var p = await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName);
            return p;
        }

        public async Task<PogoPokemon> CreatePogoPokemonFromPokeApi(int pokedexId)
        {
            PogoPokemon pogoPokemon = new PogoPokemon();

            // Get the Pokemon
            var pokeApiPokemon = await GetPokemonAsync(pokedexId);

            pogoPokemon.Id = Guid.NewGuid();
            pogoPokemon.ImageUrl = pokeApiPokemon.Sprites.FrontMale != null ? pokeApiPokemon.Sprites.FrontMale : pokeApiPokemon.Sprites.FrontFemale;
            pogoPokemon.Name = pokeApiPokemon.Name;
            pogoPokemon.PokedexNumber = pokeApiPokemon.ID;

            return pogoPokemon;
        }
    }
}
