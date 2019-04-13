using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.ProfileService.Models
{
    public class PogoProfile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserAccountId { get; set; }
        public Guid PogoTeamId { get; set; }

        public string PogoUsername { get; set; }
        public string PogoTrainerCode { get; set; }
        public int PogoLevel { get; set; }
        public Gender PlayerGender { get; set; } = Gender.None;

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
