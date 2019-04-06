using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using GakkoServices.Core.Messages;
using RawRabbit;
using RawRabbit.Context;
using GakkoServices.Core.Services;
using GakkoServices.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GakkoServices.AuthServer.Data.Contexts;
using GakkoServices.AuthServer.Business.Services;

namespace GakkoServices.AuthServer.BackgroundServices
{
    public class MessageHandlerService: BackgroundService
    {
        private IServiceScopeFactory _scopeFactory;
        private IBusClient _queue;
        private ILogger _logger;
        private AccountService _accountService;

        public MessageHandlerService(IServiceScopeFactory scopeFactory, IBusClient queue, ILoggerFactory loggerFactory, AccountService accountService)
        {
            _accountService = accountService;
            _scopeFactory = scopeFactory;
            _queue = queue;
            _logger = loggerFactory?.CreateLogger<MessageHandlerService>();
        }

        protected override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            _queue.RespondAsync<AuthenticationRequestMessage, ResultMessage>(VerifyToken);
            return Task.CompletedTask;
        }

        private async Task<ResultMessage> VerifyToken(AuthenticationRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var tokenValidationArgs = await _accountService.ValidateAuthToken(message.AuthToken);
                if (!tokenValidationArgs.IsValid) {
                     throw new Exception("Token is invalid"); 
                }

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new AuthenticationData {
                        UserId = tokenValidationArgs.Token.UserId,
                        Username = "",
                        TokenId = tokenValidationArgs.Token.Id,
                        Token = tokenValidationArgs.Token.Token
                    },
                };
            }
        }
    }
}
