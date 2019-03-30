using System;
using System.Threading.Tasks;
using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using RawRabbit;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class APIGatewayQuery : ObjectGraphType
    {
        public APIGatewayQuery(IBusClient queue)
        {
            Field<ProfileType>(
                "profile",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context => {
                    var responseTask = queue.RequestAsync<ProfileRequestMessage, ResultMessage>(
                        new ProfileRequestMessage {
                            Id = Guid.Parse(context.GetArgument<string>("id")),
                        }
                    );
                    var profileData = responseTask.Result.data as ProfileData;
                    return new Profile {
                        Username = profileData.Username,
                        TrainerCode = profileData.TrainerCode,
                        Level = profileData.Level,
                        Team = null,
                        Pokemon = null,
                    };
                }
            );
        }
    }
}