using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.AuthServer.Business.Services;
using GakkoServices.AuthServer.Models;
using GakkoServices.AuthServer.Models.UserAccount;
using GakkoServices.Core.Messages;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace GakkoServices.AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public UserAccountController(AccountService accountService, ILoggerFactory loggerFactory)
        {
            _accountService = accountService;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<UserAccountController>();
        }

        /// <summary>
        /// Creates a Brand new User in the system
        /// </summary>
        /// <param name="item">A UserCreate containing Username, Email and Password</param>
        /// <returns>A success message, or errors</returns>
        [HttpPost]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserCreate item)
        {
            var result = await _accountService.RegisterNewUser(item, true);
            if (result.Successful)
            {
                _logger.LogInformation(3, $"User({result.CreatedUser.Id}): ${item.Username}, created a new account with password.");

                // Return with a success message
                return new ObjectResult($"User was successfully created");
            }

            // There was an error
            return new ObjectResult(result);
        }
    }
}
