using System;
using System.Collections.Generic;
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
        // IBusClient is the client for the message queue (RawRabbit)
        private IBusClient _queue;

        public QueueHelpers(IBusClient queue)
        {
            _queue = queue;
        }

        // Get a profile from the graphql context (which must contain the token)
        public async Task<Profile> GetProfileFromContext(ResolveFieldContext<object> context)
        {
            Guid userId = (await AuthenticateFromContext(context)).UserId;
            return await GetProfileFromAccountId(userId);
        }

        // Get a profile from a UserID
        public async Task<Profile> GetProfileFromAccountId(Guid id)
        {
            var profileData = (await _queue.RequestAsync<ProfileRequestMessage, ResultMessage>(
                new ProfileRequestMessage {
                    UserAccountId = id,
                }
            )).data as ProfileData;
            Profile p = new Profile();
            p.Id = profileData.Id;
            p.Username = profileData.Username;
            p.TrainerCode = profileData.TrainerCode;
            p.Level = profileData.Level;
            p.TeamId = profileData.TeamId;
            p.Gender = profileData.Gender;
            p.FeaturedPokemon = new List<Guid>();
            p.FeaturedPokemon.Add(profileData.FeaturedPokemon1);
            p.FeaturedPokemon.Add(profileData.FeaturedPokemon2);
            p.FeaturedPokemon.Add(profileData.FeaturedPokemon3);
            p.FeaturedBadges = new List<Guid>();
            p.FeaturedBadges.Add(profileData.FeaturedBadge1);
            p.FeaturedBadges.Add(profileData.FeaturedBadge2);
            p.FeaturedBadges.Add(profileData.FeaturedBadge3);
            p.FeaturedBadges.Add(profileData.FeaturedBadge4);
            p.FeaturedBadges.Add(profileData.FeaturedBadge5);
            return p;
        }

        // Authenticate a token that's in the graphql context
        public Task<AuthenticationData> AuthenticateFromContext(ResolveFieldContext<object> context)
        {
            return AuthenticateToken(((dynamic) context.UserContext).token);
        }

        // Authenticate a token with the AuthServer
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