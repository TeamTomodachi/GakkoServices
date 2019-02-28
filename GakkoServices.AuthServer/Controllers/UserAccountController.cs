using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.AuthServer.Models;
using GakkoServices.AuthServer.Models.UserAccount;
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
    public class UserAccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;


        public UserAccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<UserAccountController>();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser([FromBody] UserCreate item)
        {
            //// May not be needed...
            //// Search for Existing Users
            //var existingUserName = await _userManager.FindByNameAsync(item.Username);
            //var existingEmail = await _userManager.FindByEmailAsync(item.Email);
            //if (existingUserName != null) { return new ObjectResult("There exists a user with this username in the system"); }
            //if (existingEmail != null) { return new ObjectResult("There exists a user with this email in the system"); }

            // Create the User
            var newUser = new ApplicationUser { UserName = item.Username, Email = item.Email };
            var result = await _userManager.CreateAsync(newUser, item.Password);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                _logger.LogInformation(3, $"User: ${newUser.UserName}, created a new account with password.");

                // Return with a success message
                return new ObjectResult($"User was successfully created");
            }

            // There was an error
            return new ObjectResult(result);

        }
    }
}