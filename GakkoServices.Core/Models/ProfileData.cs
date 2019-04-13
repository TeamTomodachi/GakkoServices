using System;

namespace GakkoServices.Core.Models
{
    public class ProfileData
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string TrainerCode { get; set; }
        public int Gender { get; set; }
        public Guid TeamId { get; set; }
        public int Level { get; set; }

        public Guid FeaturedPokemon1 { get; set; }
        public Guid FeaturedPokemon2 { get; set; }
        public Guid FeaturedPokemon3 { get; set; }

        public Guid FeaturedBadge1 { get; set; }
        public Guid FeaturedBadge2 { get; set; }
        public Guid FeaturedBadge3 { get; set; }
        public Guid FeaturedBadge4 { get; set; }
        public Guid FeaturedBadge5 { get; set; }
    }
}
