using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using GakkoServices.Microservices.MetadataService.Data.Contexts;
using GakkoServices.Core.Messages;
using RawRabbit;
using RawRabbit.Context;
using GakkoServices.Microservices.MetadataService.Models;
using GakkoServices.Microservices.MetadataService.Services;
using GakkoServices.Core.Services;
using GakkoServices.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GakkoServices.Microservices.MetadataService.BackgroundServices
{
    public class MessageHandlerService: BackgroundService
    {
        private IServiceScopeFactory _scopeFactory;
        private IBusClient _queue;
        private PokeApiService _poke;
        private ILogger _logger;

        public MessageHandlerService(IServiceScopeFactory scopeFactory, IBusClient queue,
                                     PokeApiService poke, ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
            _poke = poke;
            _logger = loggerFactory?.CreateLogger<MessageHandlerService>();
        }

        protected override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            _queue.RespondAsync<TeamRequestMessage, ResultMessage>(GetTeam);
            _queue.RespondAsync<TeamsRequestMessage, ResultMessage>(GetAllTeams);
            _queue.RespondAsync<PokemonRequestMessage, ResultMessage>(GetPokemon);
            _queue.RespondAsync<PokemenRequestMessage, ResultMessage>(GetAllPokemon);
            _queue.RespondAsync<BadgeRequestMessage, ResultMessage>(GetBadge);
            _queue.RespondAsync<BadgesRequestMessage, ResultMessage>(GetAllBadges);
            _queue.RespondAsync<PokemonTypeRequestMessage, ResultMessage>(GetPokemonType);
            _queue.RespondAsync<PokemonTypesRequestMessage, ResultMessage>(GetAllPokemonTypes);
            return Task.CompletedTask;
        }

        private async Task<ResultMessage> GetTeam(TeamRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                PogoTeam team = await dbContext.PogoTeams.FindAsync(message.Id);

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new TeamData {
                        Id = team.Id,
                        Name = team.Name,
                        Color = team.Color,
                        ImageUrl = team.ImageUrl,
                    },
                };
            }
        }

        private async Task<ResultMessage> GetBadge(BadgeRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                PogoBadge badge = await dbContext.PogoBadges.FindAsync(message.Id);

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new BadgeData {
                        Id = badge.Id,
                        Name = badge.Name,
                        ImageUrl = badge.ImageUrl,
                    },
                };
            }
        }

        private async Task<ResultMessage> GetPokemonType(PokemonTypeRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                PogoPokemonType type = await dbContext.PogoPokemonTypes.FindAsync(message.Id);

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new PokemonTypeData {
                        Id = type.Id,
                        Name = type.Name,
                    },
                };
            }
        }

        private async Task<ResultMessage> GetAllBadges(BadgesRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                var badges = await dbContext.PogoBadges
                    .Select(x => new BadgeData {
                        Id = x.Id,
                        Name = x.Name,
                        ImageUrl = x.ImageUrl,
                    })
                    .ToListAsync();

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = badges,
                };
            }
        }

        private async Task<ResultMessage> GetAllPokemonTypes(PokemonTypesRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                var types = await dbContext.PogoBadges
                    .Select(x => new PokemonTypeData {
                        Id = x.Id,
                        Name = x.Name,
                    })
                    .ToListAsync();

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = types,
                };
            }
        }

        private async Task<ResultMessage> GetAllTeams(TeamsRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                var teams = await dbContext.PogoTeams
                    .Select(x => new TeamData {
                        Id = x.Id,
                        Name = x.Name,
                        Color = x.Color,
                        ImageUrl = x.ImageUrl,
                    })
                    .ToListAsync();

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = teams,
                };
            }
        }

        private async Task<ResultMessage> GetPokemon(PokemonRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                PogoPokemon pokemon;
                if (message.Id.HasValue)
                {
                    _logger.LogDebug("Got pokemon ID in request message {Guid}", message.Id);
                    pokemon = await dbContext.PogoPokemon.FindAsync(message.Id);
                }
                else if (message.PokedexNumber.HasValue)
                {
                    pokemon = await dbContext.PogoPokemon
                        .Where(p => p.PokedexNumber == message.PokedexNumber.Value)
                        .FirstOrDefaultAsync();
                    if (pokemon == null)
                    {
                        _logger.LogDebug("Pokemon is null, getting by pokedexNumber from api");
                        // get from api, add to database, return
                        pokemon = await _poke.CreatePogoPokemonFromPokeApi(message.PokedexNumber.Value);
                        await dbContext.AddAsync(pokemon);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(message.Name))
                {
                    pokemon = await dbContext.PogoPokemon
                        .Where(p => p.Name.ToLower() == message.Name.ToLower())
                        .FirstOrDefaultAsync();
                    if (pokemon == null)
                    {
                        _logger.LogDebug("Pokemon is null, getting by name from api");
                        // get from api, add to database, return
                        pokemon = await _poke.CreatePogoPokemonFromPokeApi(message.Name);
                        await dbContext.AddAsync(pokemon);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else {
                    throw new ArgumentNullException();
                }

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new PokemonData {
                        Id = pokemon.Id,
                        Name = pokemon.Name,
                        PokedexNumber = pokemon.PokedexNumber,
                        SpriteImageUrl = pokemon.SpriteImageUrl,
                        PogoImageUrl = pokemon.PogoImageUrl,
                    },
                };
            }
        }

        private async Task<ResultMessage> GetAllPokemon(PokemenRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                var pokemon = await dbContext.PogoPokemon
                    .Select(x => new PokemonData {
                        Id = x.Id,
                        Name = x.Name,
                        PokedexNumber = x.PokedexNumber,
                        SpriteImageUrl = x.SpriteImageUrl,
                        PogoImageUrl = x.PogoImageUrl,
                    })
                    .ToListAsync();

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = pokemon,
                };
            }
        }
    }
}
