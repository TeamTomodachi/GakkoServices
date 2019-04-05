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
    [EnableCors]
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

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get( [FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) { throw new ArgumentNullException(nameof(query)); }

            var graphQLQuery = new GraphQLQuery(query);
            return await PreformQuery(graphQLQuery);
        }

        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            return await PreformQuery(query);
        }

        private async Task<IActionResult> PreformQuery(GraphQLQuery query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            var inputs = query.Variables.ToInputs();
            var executionOptions = new ExecutionOptions
            {
                Schema = m_schema,
                Query = query.Query,
                Inputs = inputs,
                ExposeExceptions = true,
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
