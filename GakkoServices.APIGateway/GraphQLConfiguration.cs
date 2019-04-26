using Microsoft.Extensions.DependencyInjection;
using GakkoServices.APIGateway.Models.GraphQL;
using GraphQL;

namespace GakkoServices.APIGateway
{
    // This class adds the graphql types to the DI framework as singletons
    // When adding a new GraphQL Type, it must also be added here
    public static class GraphQLConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<APIGatewayQuery>();
            services.AddSingleton<APIGatewayMutation>();
            services.AddSingleton<ProfileType>();
            services.AddSingleton<ProfileInputType>();
            services.AddSingleton<TeamType>();
            services.AddSingleton<PokemonType>();
            services.AddSingleton<PokemonTypeType>();
            services.AddSingleton<BadgeType>();
        }
    }
}
