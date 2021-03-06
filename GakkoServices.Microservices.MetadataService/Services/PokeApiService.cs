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
        public PokeApiService()
        {
            HttpBackend httpBackend = new HttpBackend("https://pokeapi.co/api/v2/", "PogoGakko");
            DataFetcher.DataBackend = httpBackend;
        }

        public async Task<PokemonSpecies> GetPokemonSpeciesAsync(int pokedexId)
        {
            var p = await DataFetcher.GetApiObject<PokemonSpecies>(pokedexId);
            return p;
        }
        public async Task<PokemonSpecies> GetPokemonSpeciesAsync(string pokemonName)
        {
            var p = await DataFetcher.GetNamedApiObject<PokemonSpecies>(pokemonName.ToLower());
            return p;
        }
        public async Task<Pokemon> GetPokemonAsync(int pokedexId)
        {
            var p = await DataFetcher.GetApiObject<Pokemon>(pokedexId);
            return p;
        }
        public async Task<Pokemon> GetPokemonAsync(string pokemonName)
        {
            var p = await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName.ToLower());
            return p;
        }

        public async Task<PogoPokemon> CreatePogoPokemonFromPokeApi(string pokemonName)
        {
            var pokeApiPokemon = await GetPokemonAsync(pokemonName);
            return await CreatePogoPokemonFromPokeApi(pokeApiPokemon.ID);
        }

        public async Task<PogoPokemon> CreatePogoPokemonFromPokeApi(int pokedexId)
        {
            // Get the Pokemon
            var pokeApiPokemon = await GetPokemonAsync(pokedexId);
            var pokeApiPokemonSpecies = await GetPokemonSpeciesAsync(pokedexId);

            // Parse out the response
            var spriteImageUrl = pokeApiPokemon.Sprites.FrontMale != null ? pokeApiPokemon.Sprites.FrontMale : pokeApiPokemon.Sprites.FrontFemale;
            var pokedexNumber = pokeApiPokemon.ID;
            var pokemonName = pokeApiPokemonSpecies.Names.Where(x => x.Language.Name == "en").Select(x => x.Name).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(pokemonName)) pokemonName = pokeApiPokemon.Name;

            // Create the PogoPokemon DB Record, for entry into the DB
            PogoPokemon pogoPokemon = new PogoPokemon();
            pogoPokemon.Id = Guid.NewGuid();
            pogoPokemon.SpriteImageUrl = spriteImageUrl;
            pogoPokemon.PogoImageUrl = CreatePogoImageUrl(pokedexNumber);
            pogoPokemon.Name = pokemonName;
            pogoPokemon.PokedexNumber = pokedexNumber;
            return pogoPokemon;
        }

        public string CreatePogoImageUrl(int pokedexNumber)
        {
            string formattedPokedexNumber = pokedexNumber.ToString("000");
            string imageLinkP1 = "https://raw.githubusercontent.com/ZeChrales/PogoAssets/master/pokemon_icons/pokemon_icon_";
            string imageLinkP2 = "_00.png";
            return imageLinkP1 + formattedPokedexNumber + imageLinkP2;
        }
    }
}
