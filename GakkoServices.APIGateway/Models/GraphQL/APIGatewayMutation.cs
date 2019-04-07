using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using RawRabbit;
using Newtonsoft.Json;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class APIGatewayMutation : ObjectGraphType
    {
        public APIGatewayMutation(IBusClient queue, QueueHelpers helpers)
        {
            FieldAsync<ProfileType>(
                "updateMe",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProfileInputType>> { Name = "profile" }
                ),
                resolve: async context =>
                {
                    Guid userId = (await helpers.AuthenticateFromContext(context)).UserId;
                    var newProfile = context.Arguments["profile"] as Dictionary<string, object>;

                    Func<string, object> getOrNull = prop => newProfile.ContainsKey(prop) ? newProfile[prop] : null;
                    Func<string, Guid?> maybeGuid = field => getOrNull(field) != null
                                                    ? (Guid?) Guid.Parse(newProfile[field] as string)
                                                    : null;

                    Guid id = (await helpers.GetProfileFromAccountId(userId)).Id;

                    var result = await queue.RequestAsync<ProfileUpdateRequestMessage, ResultMessage>(
                        new ProfileUpdateRequestMessage {
                            Id = id,
                            PogoUsername = getOrNull("username") as string,
                            PogoTeamId = maybeGuid("teamId"),
                            PogoTrainerCode = getOrNull("trainerCode") as string,
                            Gender = getOrNull("gender") as int?,
                            PogoLevel = getOrNull("level") as int?,
                            FeaturedPokemon1 = maybeGuid("featuredPokemon1"),
                            FeaturedPokemon2 = maybeGuid("featuredPokemon2"),
                            FeaturedPokemon3 = maybeGuid("featuredPokemon3"),
                        }
                    );
                    if (result.status != ResultMessage.Status.Ok) {
                        throw (Exception) result.data;
                    }

                    return await helpers.GetProfileFromAccountId(userId);
                }
            );
        }
    }
}