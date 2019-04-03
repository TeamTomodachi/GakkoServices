using System;
using System.Collections.Generic;

namespace GakkoServices.APIGateway.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int Gender { get; set; }
        public string TrainerCode { get; set; }
        public int Level { get; set; }
        public Guid TeamId { get; set; }
        public List<Guid> FeaturedPokemon { get; set; }
    }
}
