using GraphQL.Types;
using GakkoServices.Core.Services;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class PokemonType : ObjectGraphType<Pokemon>
    {
        public PokemonType(ContextServiceLocator ContextServiceLocator)
        {
            Name = "Pokemon";

            Field<StringGraphType>("id", resolve: context => context.Source.Id.ToString());
            Field(x => x.Name);
            Field(x => x.PokedexNumber);
            Field(x => x.ImageUrl);
        }
    }
}
