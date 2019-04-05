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
        public Enum Gender;
    }
}
