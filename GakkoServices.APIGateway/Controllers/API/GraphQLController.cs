using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.APIGateway.Models;
using GakkoServices.APIGateway.Models.GraphQL;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GakkoServices.APIGateway.Controllers.API
{
    // [EnableCors(Startup.CORS_POLICY)]
    [Route("api/[controller]")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        private readonly IDocumentExecuter m_documentExecuter;
        private readonly ISchema m_schema;

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
        {
            m_schema = schema;
            m_documentExecuter = documentExecuter;
        }

        [EnableCors(Startup.CORS_POLICY)]
        [HttpGet]
        /// Query the GraphQL API over GET
        public async Task<IActionResult> Get([FromQuery] string query, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(query)) { throw new ArgumentNullException(nameof(query)); }

            var graphQLQuery = new GraphQLQuery(query);
            return await PreformQuery(graphQLQuery, token);
        }

        // [EnableCors(Startup.CORS_POLICY)]
        [HttpPost]
        /// Query the GraphQL API over POST
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query, [FromHeader] string token)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            return await PreformQuery(query, token);
        }

        /// Perform the query against the schema
        private async Task<IActionResult> PreformQuery(GraphQLQuery query, string token)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                UserContext = new { token },
                Schema = m_schema,
                Query = query.Query,
                Inputs = inputs,
                // uncomment the next line to show exceptions for debugging
                // ExposeExceptions = true,
            };

            var result = await m_documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
