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
    /// This class defines the mutations (create/update actions) the client can
    /// perform through the graphql API
    public class APIGatewayMutation : ObjectGraphType
    {
        public APIGatewayMutation(IBusClient queue, QueueHelpers helpers)
        {
            // This field allows the client to update the current user's account
            // according to the token they are using.
            FieldAsync<ProfileType>(
                "updateMe",
                // As argument is a partial Profile object, see the
                // ProfileInputType class for details
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ProfileInputType>> { Name = "profile" }
                ),
                resolve: async context =>
                {
                    // Using the token in the context, authenticate the user and
                    // get their UserId
                    Guid userId = (await helpers.AuthenticateFromContext(context)).UserId;
                    // Get the ProfileInputType argument as a Dictionary
                    var newProfile = context.Arguments["profile"] as Dictionary<string, object>;

                    // Helper function for getting a value from the argument, or
                    // null if it isn't defined
                    Func<string, object> getOrNull = prop => newProfile.ContainsKey(prop) ? newProfile[prop] : null;
                    // Helper function for getting a Guid of the profile or null
                    Func<string, Guid?> maybeGuid = field => getOrNull(field) != null
                                                    ? (Guid?) Guid.Parse(newProfile[field] as string)
                                                    : null;

                    // Get the Profile ID of the account based on the UserId we
                    // got earlier
                    Guid id = (await helpers.GetProfileFromAccountId(userId)).Id;

                    // Make a request to update the Profile with the new values.
                    // Anything that is null won't be updated by the handler of
                    // the message.
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
                    // If the response doesn't have the status Ok, the data
                    // property is an exception object, so we just throw that
                    if (result.status != ResultMessage.Status.Ok) {
                        throw (Exception) result.data;
                    }

                    // Get the profile again so we have the updated values and
                    // return it so the client can get fields from it
                    return await helpers.GetProfileFromAccountId(userId);
                }
            );
        }
    }
}