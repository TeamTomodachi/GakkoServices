﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.APIGateway.Models.GraphQL
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }

        public GraphQLQuery() { }
        public GraphQLQuery(string query)
        {
            Query = query;
        }
    }
}
