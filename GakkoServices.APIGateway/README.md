# GakkoServices.APIGateway

The API Gateway is the centralized location for API Calls. It's the "Front-end, for the Back-end" and allows a centralized location to:

1. Query the Services and Gather Data
2. Perform updates on the services
3. Perform authorization before allowing the resources through

## Routing

The Base URL of the API Gateway is `/api-gateway`

The API Gateway has two graphical interpreter endpoints

* `/api-gateway/graphiql`
* `/api-gateway/swagger`

And a single API Controller

* `/api-gateway/api/graphql`

## Docker

The APIGateway is on the `traefik` Docker network, which means that it's exposed
to the internet. It's also a part of the `internal` network, which links it to
the RabbitMQ instance. As mentioned above, this application has a path prefix of
`/api-gateway`, which is not removed before it reaches the APIGateway. This
means that the APIGateway must prefix all endpoints with `/api-gateway`.