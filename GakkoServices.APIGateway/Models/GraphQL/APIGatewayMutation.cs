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
        public APIGatewayMutation(IBusClient queue)
        {
            FieldAsync<ProfileType>(
                "updateProfile",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<NonNullGraphType<ProfileInputType>> { Name = "profile" }
                ),
                resolve: async context =>
                {
                    var newProfile = context.Arguments["profile"] as Dictionary<string, object>;
                    Func<string, object> getOrNull = prop => newProfile.ContainsKey(prop) ? newProfile[prop] : null;

                    Guid id = Guid.Parse(context.GetArgument<string>("id"));
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

                    var result2 = await queue.RequestAsync<ProfileRequestMessage, ResultMessage>(
                        new ProfileRequestMessage {
                            Id = id,
                        }
                    );
                    var profileData = result2.data as ProfileData;
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
            );
        }
    }
}