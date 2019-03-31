using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GakkoServices.Microservices.ProfileService.Data.Contexts;
using GakkoServices.Core.Messages;
using RawRabbit;
using RawRabbit.Context;
using GakkoServices.Microservices.ProfileService.Models;
using GakkoServices.Core.Services;
using GakkoServices.Core.Models;

namespace GakkoServices.Microservices.ProfileService.BackgroundServices
{
    public class ProfileMessageHandlerService: BackgroundService
    {
        private IServiceScopeFactory _scopeFactory;
        private IBusClient _queue;

        public ProfileMessageHandlerService(IServiceScopeFactory scopeFactory, IBusClient queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            _queue.SubscribeAsync<UserCreateMessage>(CreateProfile);
            _queue.RespondAsync<ProfileRequestMessage, ResultMessage>(GetProfile);
            _queue.RespondAsync<ProfileUpdateRequestMessage, ResultMessage>(UpdateProfile);
            return Task.CompletedTask;
        }

        private async Task<ResultMessage> GetProfile(ProfileRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProfileServiceDbContext>();
                var profile = await dbContext.PogoProfiles.FindAsync(message.Id);

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new ProfileData {
                        Id = profile.Id,
                        Username = profile.PogoUsername,
                        TrainerCode = profile.PogoTrainerCode,
                        Level = profile.PogoLevel,
                        TeamId = profile.PogoTeamId,
                    },
                };
            }
        }

        private async Task CreateProfile(UserCreateMessage message, MessageContext context)
        {
            var profile = new PogoProfile
            {
                Id = message.Id,
                UserAccountId = message.Id,
                PogoUsername = "anonymous",
                PogoLevel = 1,
                PogoTrainerCode = "0000 0000 0000",
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProfileServiceDbContext>();
                await dbContext.AddAsync(profile);
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task<ResultMessage> UpdateProfile(ProfileUpdateRequestMessage message, MessageContext context)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ProfileServiceDbContext>();
                    var profile = await dbContext.FindAsync<PogoProfile>(message.Id);

                    if (message.PogoLevel.HasValue) {
                        profile.PogoLevel = message.PogoLevel.Value;
                    }
                    if (message.PogoTeamId.HasValue) {
                        profile.PogoTeamId = message.PogoTeamId.Value;
                    }
                    if (message.PogoUsername != null) {
                        profile.PogoUsername = message.PogoUsername;
                    }
                    await dbContext.SaveChangesAsync();
                }

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                };
            }
            catch (Exception e)
            {
                return new ResultMessage {
                    status = ResultMessage.Status.Error,
                    data = e,
                };
            }
        }
    }
}
