using System;

namespace GakkoServices.Core.Models
{
    public class ProfileData
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string TrainerCode { get; set; }
        public Enum Gender { get; set; }
        public Guid TeamId { get; set; }
        public int Level { get; set; }
        public Guid FeaturedPokemon1 { get; set; }
        public Guid FeaturedPokemon2 { get; set; }
        public Guid FeaturedPokemon3 { get; set; }
    }
}
