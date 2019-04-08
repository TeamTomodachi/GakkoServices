using GraphQL.Types;
using GakkoServices.Core.Services;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class PokemonTypeType : ObjectGraphType<Pokemon>
    {
        public PokemonTypeType(ContextServiceLocator ContextServiceLocator)
        {
            Name = "PokemonType";

            Field<StringGraphType>("id", resolve: context => context.Source.Id.ToString());
            Field(x => x.Name);
        }
    }
}
