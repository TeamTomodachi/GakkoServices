using GraphQL.Types;
using GakkoServices.Core.Services;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class TeamType : ObjectGraphType<Team>
    {
        public TeamType(ContextServiceLocator contextServiceLocator)
        {
            Name = "Team";

            Field<StringGraphType>("id", resolve: context => context.Source.Id.ToString());
            Field(x => x.Name);
            Field(x => x.Color);
            Field(x => x.ImageUrl);
        }
    }
}
