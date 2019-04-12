using System;

namespace GakkoServices.Core.Messages
{
    public class TeamRequestMessage
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }
}
