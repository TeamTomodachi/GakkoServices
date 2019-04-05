using GraphQL.Types;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class ProfileInputType : InputObjectGraphType
    {
        public ProfileInputType()
        {
            Name = "ProfileInput";
            Field<StringGraphType>("username");
            Field<StringGraphType>("trainerCode");
            Field<IntGraphType>("gender");
            Field<StringGraphType>("teamId");
            Field<StringGraphType>("featuredPokemon1");
            Field<StringGraphType>("featuredPokemon2");
            Field<StringGraphType>("featuredPokemon3");
        }
    }
}