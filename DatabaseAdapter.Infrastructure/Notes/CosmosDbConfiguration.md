
# Cosmos DB Configuration for Local Development

## 1. Running Cosmos DB Emulator Locally

When running the Cosmos DB Emulator locally, you'll need to either pass a valid certificate or configure the Cosmos DB client to bypass certificate validation. This is particularly useful for local testing where you might not have a production-grade certificate.

Here’s an example of how to configure the `CosmosClientOptions` to ignore the certificate validation for local development:

```csharp
private const string AccountEndpoint = "https://localhost:8081/";
private const string AccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

CosmosClientOptions _options = new CosmosClientOptions
{
    HttpClientFactory = () =>
    {
        HttpMessageHandler httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
        };
        return new HttpClient(httpMessageHandler);
    },
    ApplicationName = "CosmosDBDotnetQuickstart", // Customize with your application name
    ConnectionMode = ConnectionMode.Gateway,       // Use Gateway mode when working locally or with emulator
    LimitToEndpoint = true                         // Limits connections to the specified endpoint for efficiency
};

// Pass this options object when initializing your CosmosClient.
```

> **Note**: The above configuration disables SSL certificate validation. This is acceptable in local environments with the Cosmos DB emulator, but you should **never** use this in production.

## 2. Accessing the Cosmos DB Emulator

Once the emulator is running, you can access the Data Explorer to manage your database through a web interface:

- **URL**: [https://localhost:8081/_explorer/index.html](https://localhost:8081/_explorer/index.html)  
- **Wait Time**: It may take 1-2 minutes after starting the emulator for the web interface to become available.

## 3. Example Docker Compose Configuration

If you're using Docker, here’s a sample `docker-compose.yml` configuration to run the Cosmos DB Emulator in a container:

```yaml
services:
  cosmos-db-emulator:
    image: mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest
    container_name: cosmos-db-emulator
    ports:
      - "8081:8081"
      - "10250-10255:10250-10255"
    environment:
      AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE: "true"
      AZURE_COSMOS_EMULATOR_PARTITION_COUNT: "1"
```

This setup allows you to use the Cosmos DB Emulator in a containerized environment, making it easier to work with in development or testing scenarios.
