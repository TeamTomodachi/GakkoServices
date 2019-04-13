using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GakkoServices.Microservices.ProfileService.Models
{
    public class PogoProfileBadges
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid BadgeId { get; set; }

        [Required]
        [ForeignKey(nameof(Profile))]
        public Guid ProfileId { get; set; }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual PogoProfile Profile { get; set; }
    }
}
