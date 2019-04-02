using System;

namespace GakkoServices.Core.Messages
{
    public class ProfileRequestMessage
    {
        public Guid? Id { get; set; }
        public Guid? UserAccountId { get; set; }
    }
}