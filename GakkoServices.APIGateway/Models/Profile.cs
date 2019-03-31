using System;
using System.Collections.Generic;

namespace GakkoServices.APIGateway.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string TrainerCode { get; set; }
        public int Level { get; set; }
        public Guid TeamId { get; set; }
        public Guid FeaturedPokemon1 { get; set; }
        public Guid FeaturedPokemon2 { get; set; }
        public Guid FeaturedPokemon3 { get; set; }
    }
}
