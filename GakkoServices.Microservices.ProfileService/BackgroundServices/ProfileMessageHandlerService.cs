using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using GakkoServices.Microservices.ProfileService.Data.Contexts;
using GakkoServices.Core.Messages;
using RawRabbit;
using RawRabbit.Context;
using GakkoServices.Microservices.ProfileService.Models;

namespace GakkoServices.Microservices.ProfileService.BackgroundServices
{
    public class ProfileMessageHandlerService: BackgroundService
    {
        private ProfileServiceDbContext _context;
        private IBusClient _queue;

        public ProfileMessageHandlerService(ProfileServiceDbContext context, IBusClient queue)
        {
            _context = context;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                _queue.SubscribeAsync<UserCreateMessage>(CreateProfile);
                _queue.RespondAsync<ProfileUpdateRequestMessage, ResultMessage>(UpdateProfile);
            });
        }

        private async Task CreateProfile(UserCreateMessage message, MessageContext context)
        {
            var profile = new PogoProfile {
                Id = message.Id,
                PogoUsername = "user123",
                PogoLevel = 1,
            };

            await _context.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        private async Task<ResultMessage> UpdateProfile(ProfileUpdateRequestMessage message, MessageContext context)
        {
            var profile = await _context.FindAsync<PogoProfile>(message.Id);

            if (message.PogoLevel.HasValue) {
                profile.PogoLevel = message.PogoLevel.Value;
            }
            if (message.PogoTeamId.HasValue) {
                profile.PogoTeamId = message.PogoTeamId.Value;
            }
            if (message.PogoUsername != null) {
                profile.PogoUsername = message.PogoUsername;
            }
            await _context.SaveChangesAsync();

            return new ResultMessage {
                status = ResultMessage.Status.Ok,
            };
        }
    }
}
