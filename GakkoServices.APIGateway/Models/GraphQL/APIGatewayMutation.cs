using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using RawRabbit;

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

                    Guid id = (await helpers.GetProfileFromAccountId(userId)).Id;

                    var result = await queue.RequestAsync<ProfileUpdateRequestMessage, ResultMessage>(
                        new ProfileUpdateRequestMessage {
                            Id = id,
                            PogoUsername = getOrNull("userName") as string,
                            PogoTeamId = getOrNull("teamId") != null
                                       ? (Guid?) Guid.Parse(newProfile["teamId"] as string)
                                       : null,
                            PogoTrainerCode = getOrNull("trainerCode") as string,
                            PogoLevel = getOrNull("level") as int?,
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