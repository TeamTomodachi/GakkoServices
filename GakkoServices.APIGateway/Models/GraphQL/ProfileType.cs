using GraphQL.Types;
using GakkoServices.Core.Services;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class ProfileType : ObjectGraphType<Profile>
    {
        public ProfileType(ContextServiceLocator ContextServiceLocator)
        {
            Name = "Profile";

            Field(x => x.Username);
            Field(x => x.Level);
            Field(x => x.TrainerCode);
            //Field<TeamType>("Team", );
        }
    }
}