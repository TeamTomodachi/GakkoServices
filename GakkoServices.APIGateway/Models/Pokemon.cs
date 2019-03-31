using System;

namespace GakkoServices.APIGateway.Models
{
    public class Pokemon {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PokedexNumber { get; set; }
        public string ImageUrl { get; set; }
    }
}
