using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GakkoServices.Microservices.MetadataService.Data.Contexts;
using GakkoServices.Core.Messages;
using RawRabbit;
using RawRabbit.Context;
using GakkoServices.Microservices.MetadataService.Models;
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

        public MessageHandlerService(IServiceScopeFactory scopeFactory, IBusClient queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            _queue.RespondAsync<TeamRequestMessage, ResultMessage>(GetTeam);
            _queue.RespondAsync<TeamsRequestMessage, ResultMessage>(GetAllTeams);
            _queue.RespondAsync<PokemonRequestMessage, ResultMessage>(GetPokemon);
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

        private async Task<ResultMessage> GetAllTeams(TeamsRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                var teams = await dbContext.PogoTeams.ToListAsync();

                var teamDatas = new List<TeamData>();
                foreach (var team in teams) {
                    teamDatas.Add(new TeamData {
                        Id = team.Id,
                        Name = team.Name,
                        Color = team.Color,
                        ImageUrl = team.ImageUrl,
                    });
                }

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = teamDatas,
                };
            }
        }

        private async Task<ResultMessage> GetPokemon(PokemonRequestMessage message, MessageContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                PogoPokemon pokemon;
                if (message.Id != null)
                {
                    pokemon = await dbContext.PogoPokemon.FindAsync(message.Id);
                }
                else
                {
                    pokemon = await dbContext.PogoPokemon
                        .Where(p => p.PokedexNumber == message.PokedexNumber.Value)
                        .FirstAsync();
                }

                return new ResultMessage {
                    status = ResultMessage.Status.Ok,
                    data = new PokemonData {
                        Id = pokemon.Id,
                        Name = pokemon.Name,
                        PokedexNumber = pokemon.PokedexNumber,
                        ImageUrl = pokemon.ImageUrl,
                    },
                };
            }
        }
    }
}
