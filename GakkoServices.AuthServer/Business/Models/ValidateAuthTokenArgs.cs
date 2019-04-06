using GakkoServices.AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.AuthServer.Business.Models
{
    public class ValidateAuthTokenArgs
    {
        public AuthToken Token { get; set; }
        public Guid? UserId { get { return Token?.UserId; } }
        public bool IsValid { get; set; }

        public ValidateAuthTokenArgs(AuthToken token, bool isValid)
        {
            Token = token;
            IsValid = isValid;
        }
    }
}
