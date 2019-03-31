using System;

namespace GakkoServices.Core.Models
{
    public class PokemonData
    {
        public Guid Id { get; set; }

        public int PokedexNumber { get; set; }

        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
