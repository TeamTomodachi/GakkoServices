using GraphQL.Types;
using GakkoServices.Core.Services;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class BadgeType : ObjectGraphType<Badge>
    {
        public BadgeType(ContextServiceLocator contextServiceLocator)
        {
            Name = "Badge";

            Field<StringGraphType>("id", resolve: context => context.Source.Id.ToString());
            Field(x => x.Name);
            Field(x => x.ImageUrl);
        }
    }
}
