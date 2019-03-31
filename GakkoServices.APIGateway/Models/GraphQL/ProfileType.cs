using GraphQL.Types;
using GakkoServices.Core.Services;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Models;
using RawRabbit;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class ProfileType : ObjectGraphType<Profile>
    {
        public ProfileType(ContextServiceLocator ContextServiceLocator, IBusClient queue)
        {
            Name = "Profile";

            Field<StringGraphType>("id", resolve: context => context.Source.Id.ToString());
            Field(x => x.Username);
            Field(x => x.Level);
            Field(x => x.TrainerCode);
            Field<StringGraphType>("teamId", resolve: context => context.Source.TeamId.ToString());
            Field<TeamType>(
                "team",
                resolve: context => {
                    var responseTask = queue.RequestAsync<TeamRequestMessage, ResultMessage>(
                        new TeamRequestMessage {
                            Id = context.Source.TeamId,
                        }
                    );
                    var teamData = responseTask.Result.data as TeamData;
                    return new Team {
                        Id = teamData.Id,
                        Name = teamData.Name,
                        Color = teamData.Color,
                        ImageUrl = teamData.ImageUrl,
                    };
                }
            );
        }
    }
}
