using System;
using System.Linq;
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
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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
            Console.WriteLine(JsonConvert.SerializeObject(message));
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProfileServiceDbContext>();

                PogoProfile profile = null;
                if (message.Id.HasValue) {
                    // profile = await dbContext.PogoProfiles.FindAsync(message.Id);
                    profile = await dbContext.PogoProfiles.Where(x => x.Id == message.Id).FirstOrDefaultAsync();
                }
                else if (message.UserAccountId.HasValue) {
                    profile = await dbContext.PogoProfiles.Where(x => x.UserAccountId == message.UserAccountId).FirstOrDefaultAsync();
                }
                else if (!string.IsNullOrWhiteSpace(message.Username))
                {
                    profile = await dbContext.PogoProfiles.Where(x => x.PogoUsername.ToLower() == message.Username.ToLower()).FirstOrDefaultAsync();
                }
                else {
                    throw new ArgumentNullException("GetProfile called with no Id, UserAccountId or Username");
                }

                if (profile == null) {
                    throw new NullReferenceException("Profile could not be found");
                }

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new ProfileData {
                        Id = profile.Id,
                        Username = profile.PogoUsername,
                        TrainerCode = profile.PogoTrainerCode,
                        Level = profile.PogoLevel,
                        TeamId = profile.PogoTeamId,
                        Gender = (int)profile.PlayerGender,
                        FeaturedPokemon1 = profile.FeaturedPokemon1,
                        FeaturedPokemon2 = profile.FeaturedPokemon2,
                        FeaturedPokemon3 = profile.FeaturedPokemon3,
                        FeaturedBadge1 = profile.FeaturedBadge1,
                        FeaturedBadge2 = profile.FeaturedBadge2,
                        FeaturedBadge3 = profile.FeaturedBadge3,
                        FeaturedBadge4 = profile.FeaturedBadge4,
                        FeaturedBadge5 = profile.FeaturedBadge5,
                    },
                };
            }
        }

        private async Task CreateProfile(UserCreateMessage message, MessageContext context)
        {
            // Get Pokemon
            Func<int, Task<ResultMessage>> getByPokedex = (int num) => 
                _queue.RequestAsync<PokemonRequestMessage, ResultMessage>(
                    new PokemonRequestMessage { PokedexNumber = num });

            var pokemonResults = (await Task.WhenAll(new Task<ResultMessage>[] {
                getByPokedex(1),
                getByPokedex(2),
                getByPokedex(3),
            })).Select(result => result.data as PokemonData).ToArray();
            var team = await _queue.RequestAsync<TeamRequestMessage, ResultMessage>(
                new TeamRequestMessage {
                    Name = "Mystic",
                }
            );

            var profile = new PogoProfile
            {
                Id = Guid.NewGuid(),
                UserAccountId = message.Id,
                PogoUsername = message.Username, //"anonymous",
                PogoLevel = 1,
                PogoTrainerCode = "0000 0000 0000",
                PogoTeamId = (team.data as TeamData).Id,
                PlayerGender = Gender.None,
                FeaturedPokemon1 = pokemonResults[0].Id,
                FeaturedPokemon2 = pokemonResults[1].Id,
                FeaturedPokemon3 = pokemonResults[2].Id,
                FeaturedBadge1 = Guid.Empty,
                FeaturedBadge2 = Guid.Empty,
                FeaturedBadge3 = Guid.Empty,
                FeaturedBadge4 = Guid.Empty,
                FeaturedBadge5 = Guid.Empty,
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
                    if (!string.IsNullOrWhiteSpace(message.PogoTrainerCode)) {
                        profile.PogoTrainerCode = message.PogoTrainerCode;
                    }
                    if (!string.IsNullOrWhiteSpace(message.PogoUsername)) {
                        profile.PogoUsername = message.PogoUsername;
                    }
                    if (message.FeaturedPokemon1.HasValue) {
                        profile.FeaturedPokemon1 = message.FeaturedPokemon1.Value;
                    }
                    if (message.FeaturedPokemon2.HasValue) {
                        profile.FeaturedPokemon2 = message.FeaturedPokemon2.Value;
                    }
                    if (message.FeaturedPokemon3.HasValue) {
                        profile.FeaturedPokemon3 = message.FeaturedPokemon3.Value;
                    }
                    if (message.FeaturedBadge1.HasValue) {
                        profile.FeaturedBadge1 = message.FeaturedBadge1.Value;
                    }
                    if (message.FeaturedBadge2.HasValue) {
                        profile.FeaturedBadge2 = message.FeaturedBadge2.Value;
                    }
                    if (message.FeaturedBadge3.HasValue) {
                        profile.FeaturedBadge3 = message.FeaturedBadge3.Value;
                    }
                    if (message.FeaturedBadge4.HasValue) {
                        profile.FeaturedBadge4 = message.FeaturedBadge4.Value;
                    }
                    if (message.FeaturedBadge5.HasValue) {
                        profile.FeaturedBadge5 = message.FeaturedBadge5.Value;
                    }
                    if (message.Gender.HasValue) {
                        profile.PlayerGender = (Gender) message.Gender.Value;
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
