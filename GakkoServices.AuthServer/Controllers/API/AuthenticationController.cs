using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.AuthServer.Models;
using GakkoServices.AuthServer.Models.Authentication;
using GakkoServices.AuthServer.Models.UserAccount;
using GakkoServices.Core.Helpers;
using IdentityServer4.Events;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GakkoServices.AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly ILogger _logger;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _logger = loggerFactory.CreateLogger<UserAccountController>();
        }

        /// <summary>
        /// Attempts to sign a user into the system
        /// </summary>
        /// <param name="item">A UserLogin containing Username and Password</param>
        /// <returns>The user login token or errors</returns>
        [HttpPost]
        public async Task<IActionResult> LoginCredentials([FromBody] UserLogin item)
        {
            // TODO: Look into implementing a Lockout
            var result = await _signInManager.PasswordSignInAsync(item.Username, item.Password, true, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(item.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                var userClaims = await _userManager.GetClaimsAsync(user);
                return new ObjectResult(userClaims);
            }

            await _events.RaiseAsync(new UserLoginFailureEvent(item.Username, "invalid credentials"));
            return new ObjectResult(AccountOptions.InvalidCredentialsErrorMessage);

        }
    }
}