# API Gateway
The API Gateway is the centralized location where API Calls are made. It is the "Front-end, for the Back-end" and allows a centralized location to:

1. Query the Services and Gather Data
2. Preform updates on the services
3. Preform authorization before the resources are allowed through

## Routing
The Base URL of the API Gateway can be found at `/api-gateway`

The API Gateway has two graphical interpreter endpoints

* `/api-gateway/graphiql`
* `/api-gateway/swagger`

And a single API Controller

* `/api-gateway/api/graphql`