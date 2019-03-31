using System;

namespace GakkoServices.Core.Messages
{
    public class PokemonRequestMessage
    {
        public Guid? Id { get; set; }
        public int? PokedexNumber { get; set; }
        public string Name { get; set; }
    }
}
