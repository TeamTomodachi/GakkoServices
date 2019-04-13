using System;

namespace GakkoServices.Core.Messages
{
    public class ProfileUpdateRequestMessage
    {
        // TODO: Identifier and partial profile object maybe?

        public Guid Id;
        public Guid? PogoTeamId;

        public string PogoUsername;
        public string PogoTrainerCode;
        public int? PogoLevel;
        public int? Gender;

        public Guid? FeaturedPokemon1;
        public Guid? FeaturedPokemon2;
        public Guid? FeaturedPokemon3;

        public Guid? FeaturedBadge1;
        public Guid? FeaturedBadge2;
        public Guid? FeaturedBadge3;
        public Guid? FeaturedBadge4;
        public Guid? FeaturedBadge5;
    }
}
