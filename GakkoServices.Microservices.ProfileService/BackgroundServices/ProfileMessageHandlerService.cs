using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using GakkoServices.Microservices.ProfileService.Data.Contexts;
using GakkoServices.Core.Messages;
using RawRabbit;
using RawRabbit.Context;

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

        protected override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            _queue.SubscribeAsync<UserCreateMessage>(CreateProfile);
            _queue.SubscribeAsync<ProfileUpdateRequestMessage>(UpdateProfile);
            return Task.CompletedTask;
        }

        private async Task CreateProfile(UserCreateMessage message, MessageContext context)
        {
            // do stuff
        }

        private async Task UpdateProfile(ProfileUpdateRequestMessage message, MessageContext context)
        {

        }
    }
}