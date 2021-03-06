﻿using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class APIGatewaySchema : Schema
    {
        public APIGatewaySchema(IDependencyResolver resolver) : base(resolver)
        {
            // add the query and mutation definitions to the schema
            Query = resolver.Resolve<APIGatewayQuery>();
            Mutation = resolver.Resolve<APIGatewayMutation>();
        }
    }
}
