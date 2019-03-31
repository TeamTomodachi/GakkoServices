using System;

namespace GakkoServices.Core.Models
{
    public class TeamData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }
        public string ImageUrl { get; set; }
    }
}
