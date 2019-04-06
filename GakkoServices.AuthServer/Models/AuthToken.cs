using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.AuthServer.Models
{
    public class AuthToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime LoginDateTimeUtc { get; set; }

        public DateTime? ExpiryDateTimeUtc { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public Guid UserId { get; set; }

        [NotMapped]
        public virtual ApplicationUser User { get; set; }
    }
}
