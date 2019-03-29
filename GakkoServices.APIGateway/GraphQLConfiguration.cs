using Microsoft.Extensions.DependencyInjection;
using GakkoServices.APIGateway.Models.GraphQL;
using GraphQL;

namespace GakkoServices.APIGateway
{
    public static class GraphQLConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<APIGatewayQuery>();
            services.AddSingleton<APIGatewayMutation>();
            services.AddSingleton<ProfileType>();
            services.AddSingleton<TeamType>();
        }
    }
}