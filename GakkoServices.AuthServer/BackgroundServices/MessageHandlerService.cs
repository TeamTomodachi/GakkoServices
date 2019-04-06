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

namespace GakkoServices.AuthServer.BackgroundServices
{
    public class MessageHandlerService: BackgroundService
    {
        private IServiceScopeFactory _scopeFactory;
        private IBusClient _queue;
        private ILogger _logger;

        public MessageHandlerService(IServiceScopeFactory scopeFactory, IBusClient queue, ILoggerFactory loggerFactory)
        {
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
                var dbContext = scope.ServiceProvider.GetRequiredService<AspIdentityDbContext>();
                var token = await dbContext.AuthTokens
                    .Where(x => x.Token == message.AuthToken)
                    .FirstOrDefaultAsync();
                var currentUtc = DateTime.UtcNow;

                if (token == null || (token.ExpiryDateTimeUtc != null && token.ExpiryDateTimeUtc < currentUtc)) {
                     throw new Exception("Token is invalid"); 
                } 

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new AuthenticationData {
                        UserId = token.UserId,
                        Username = "",
                        TokenId = token.Id,
                        Token = token.Token
                    },
                };
            }
        }
    }
}
