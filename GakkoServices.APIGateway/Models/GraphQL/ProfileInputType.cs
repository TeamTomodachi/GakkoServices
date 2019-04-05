using GraphQL.Types;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class ProfileInputType : InputObjectGraphType
    {
        public ProfileInputType()
        {
            Name = "ProfileInput";
            Field<StringGraphType>("userName");
            Field<StringGraphType>("trainerCode");
            Field<IntGraphType>("gender");
            Field<StringGraphType>("teamId");
            Field<ListGraphType<PokemonType>>("pokemen");
        }
    }
}