using System;
using RawRabbit;
using System.Threading.Tasks;
using GraphQL.Types;
using GakkoServices.Core.Models;
using GakkoServices.Core.Messages;
using GakkoServices.APIGateway.Models;

namespace GakkoServices.APIGateway
{
    public class QueueHelpers
    {
        private IBusClient _queue;

        public QueueHelpers(IBusClient queue)
        {
            _queue = queue;
        }

        public async Task<Profile> GetProfileFromContext(ResolveFieldContext<object> context)
        {
            Guid userId = (await AuthenticateFromContext(context)).UserId;
            return await GetProfileFromAccountId(userId);
        }

        public async Task<Profile> GetProfileFromAccountId(Guid id)
        {
            var profileData = (await _queue.RequestAsync<ProfileRequestMessage, ResultMessage>(
                new ProfileRequestMessage {
                    UserAccountId = id,
                }
            )).data as ProfileData;
            return new Profile {
                Id = profileData.Id,
                Username = profileData.Username,
                TrainerCode = profileData.TrainerCode,
                Level = profileData.Level,
                TeamId = profileData.TeamId,
                FeaturedPokemon = {
                    profileData.FeaturedPokemon1,
                    profileData.FeaturedPokemon2,
                    profileData.FeaturedPokemon3,
                },
            };
        }

        public Task<AuthenticationData> AuthenticateFromContext(ResolveFieldContext<object> context)
        {
            return AuthenticateToken(((dynamic) context.UserContext).token);
        }

        public async Task<AuthenticationData> AuthenticateToken(string token)
        {
            var result = await _queue.RequestAsync<AuthenticationRequestMessage, ResultMessage>(
                new AuthenticationRequestMessage {
                    AuthToken = token,
                }
            );

            if (result.status != ResultMessage.Status.Ok)
            {
                throw new Exception();
            }

            return result.data as AuthenticationData;
        }
    }
}