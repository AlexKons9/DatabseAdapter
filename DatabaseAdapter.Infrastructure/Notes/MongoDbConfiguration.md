
# MongoDB Configuration for Local Development

This guide will walk you through setting up MongoDB for local development using Docker, along with an example of configuring a MongoDB client connection and accessing the Mongo Express UI.

## 1. Running MongoDB Locally with Docker

You can easily run MongoDB locally by using Docker. The `docker-compose.yml` configuration file provided below will set up both MongoDB and Mongo Express, a web-based MongoDB admin interface.

### Docker Compose Setup

```yaml
services:

  mongo:
    image: mongo
    restart: always
    ports:
      - 27017:27017
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: example

  mongo-express:
    image: mongo-express
    restart: always
    ports:
      - 8081:8081
    environment:
      ME_CONFIG_MONGODB_ADMINUSERNAME: root
      ME_CONFIG_MONGODB_ADMINPASSWORD: example
      ME_CONFIG_MONGODB_URL: mongodb://root:example@mongo:27017/
      ME_CONFIG_BASICAUTH: false
```

### Steps to Run the MongoDB and Mongo Express Containers

1. **Install Docker**: If Docker is not already installed, download and install it from the [Docker website](https://www.docker.com/get-started).
2. **Create `docker-compose.yml`**: Save the above configuration into a `docker-compose.yml` file.
3. **Run the Containers**: In the directory where the `docker-compose.yml` is located, run the following command to start the MongoDB and Mongo Express containers:

```bash
docker-compose up -d
```

This command will start both MongoDB (on port `27017`) and Mongo Express (on port `8081`).

### Stopping the Containers

To stop the containers, use the following command:

```bash
docker-compose down
```

---

## 2. Accessing MongoDB and Mongo Express

### Access MongoDB

- **Port**: MongoDB will be running on `localhost:27017`. 
- **Default Credentials**:
  - Username: `root`
  - Password: `example`

You can connect to MongoDB using these credentials in your application or a MongoDB client like [MongoDB Compass](https://www.mongodb.com/products/compass).

### Access Mongo Express

Once the Mongo Express container is running, you can access its web interface via the following URL:

- **URL**: [http://localhost:8081](http://localhost:8081)

This web interface allows you to interact with your MongoDB instance, create databases, collections, and manage your data.

---

## 3. MongoDB Client Configuration in .NET

To connect your .NET application to MongoDB running locally, you can use the MongoDB .NET driver. Here’s an example of how to set up the connection.

### Step 1: Add MongoDB NuGet Package

In your .NET project, add the MongoDB.Driver package:

```bash
dotnet add package MongoDB.Driver
```

### Step 2: Configure MongoDB Client in .NET

Below is an example of how to configure the `MongoClient` in .NET to connect to MongoDB using the connection string:

```csharp
using MongoDB.Driver;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService()
    {
        var connectionString = "mongodb://root:example@localhost:27017"; // Connection string for local MongoDB
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("myDatabase"); // Replace "myDatabase" with your database name
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
```

### Note: 
Make sure to replace `myDatabase` with the actual database name you want to connect to.

---

## 4. MongoDB Initialization

If you want to initialize your MongoDB with specific collections or data when the container starts, you can add an initialization script to your Docker Compose setup.

### Example MongoDB Initialization Script

You can mount a directory containing `.js` scripts that MongoDB will automatically run at startup. Here's an updated version of the `docker-compose.yml` to include an initialization script:

```yaml
services:
  mongo:
    image: mongo
    restart: always
    ports:
      - 27017:27017
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: example
    volumes:
      - ./init-mongo.js:/docker-entrypoint-initdb.d/init-mongo.js
```


## 5. Configuration Tips

- **Data Persistence**: If you want your MongoDB data to persist after restarting the container, you should mount a volume to persist data outside the container. Here’s an example of how to add data persistence:

```yaml
volumes:
  mongo-data:
    driver: local

services:
  mongo:
    image: mongo
    restart: always
    ports:
      - 27017:27017
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: example
    volumes:
      - mongo-data:/data/db
```

- **MongoDB Compass**: You can use MongoDB Compass, the official MongoDB GUI, to connect to the running MongoDB instance on `mongodb://localhost:27017`.

---

## 6. Resources

- [MongoDB Documentation](https://docs.mongodb.com/)
- [Mongo Express Documentation](https://github.com/mongo-express/mongo-express)
- [Docker Hub - MongoDB Image](https://hub.docker.com/_/mongo)
- [Docker Hub - Mongo Express Image](https://hub.docker.com/_/mongo-express)

---

This guide provides a basic setup for running MongoDB locally in a Docker environment, along with access to Mongo Express for managing your MongoDB instance. Adjustments can be made to the `docker-compose.yml` file based on your specific development needs.
