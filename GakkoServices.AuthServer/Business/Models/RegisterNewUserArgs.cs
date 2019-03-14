using GakkoServices.AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.AuthServer.Business.Models
{
    public class RegisterNewUserArgs
    {
        public IdentityResult Result { get; set; }
        public ApplicationUser CreatedUser { get; set; }
        public bool Successful { get { return Result.Succeeded; } }

        public RegisterNewUserArgs(IdentityResult result, ApplicationUser createdUser)
        {
            Result = result;
            CreatedUser = createdUser;
        }
    }
}
