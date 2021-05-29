# Microservices-Dapr-NetCore

This repo showcases microservices development using .Net Core and Dapr

Microservices With Dapr Flow Diagram:
![alt text](docs/Microservices-Dapr.png 'Microservices With Dapr Flow Diagram')

# Overview

1. Faces Web MVC sends the request to create an Order
2. Order API creates and stores the order in SQL Server DB
3. Notification Service sends email to customer regarding order updates

# Dapr Building Blocks Used

1. Service Invocation
2. Pub Sub
3. StateStore
4. Actors

# Dapr

1. Faces Web MVC sends request to Envoy API Gateway to create an Order
2. Envoy identifies the prefix **_/o/_** and forwards the request to Dapr Side Car of Orders API
3. Orders API creates an Order by consuimg Faces API by using Dapr Service Invocation
4. Orders API publishes Order Processed event to Dapr Pub/Sub side car
5. Dapr registers the event, using **_pubsub.yaml_**, in RabbitMQ
6. Notification API subscribes to Order Processed Event through Dapr Side Car
7. Notification API publishes Order Dispatched Event through Dapr Side Car
8. Orders API subscribes to Order Dispatched Event through Dapr Side Car
9. Orders API and Notifcation API implements Order Status consistency check using Dapr StateStore via Actors.

# Run the application

1. Use Visual Studio to get the best **_F5_** debugging experience.
2. To start, open the FacesAPI.sln solution file in Visual Studio.
3. The solution contains a **_Docker Compose_** project. Make sure it's set as the **default startup project**. Right-click on the docker-compose node in the Project Explorer, and select the Set as StartUp Project menu option.
4. Now you can build and run the application by pressing **_Ctrl+F5 or start debugging by pressing F5_**.

# Run the application from CLI

1. The root folder of the repository contains Docker Compose files to run the solution locally.
2. The **_docker-compose.yml_** file contains the definition of all the images needed to run Faces API.
3. The **_docker-compose.override.yml_** file contains the base configuration for all images of the previous file.

To start **_FacesAPI_** from the CLI, run the following command from the root folder:

```
docker-compose up
```

# Run the application in self-hosted mode

In self-hosted mode everything will run on your local machine. To prevent port-collisions, all services listen on a different HTTP port. When running the services with Dapr, you need additional ports for HTTP and gRPC communication with the sidecars. By default these ports are 3500 and 50001. The services will use the following ports:

| Services      | App Port |
| ------------- | :------: |
| Orders API    |   5000   |
| Faces API     |   6000   |
| Faces Web MVC |   7000   |
| Envoy API GW  |   5202   |
| Sql Server    |   1445   |
| Rabbit MQ     |   5672   |

Before running the dapr commands, make sure -

1. run Sql Server and RabbitMQ in Docker:
2. Dapr is installed and initialized (dapr init)
3. Dapr placement container is running

Run Sql Server -

```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Pass@word" --name sqldata -p 1445:1433 mcr.microsoft.com/mssql/server:latest
```

Run RabbitMQ -

```
docker run --name rabbitmq -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

Now, change the following files -

1. Orders API -> appSettings.json -> OrdersContext

```
"OrdersContext": "Server=localhost,1445;Database=OrdersDb;User Id=sa;password=Pass@word"
```

2. dapr -> Components -> pubsub.yaml

```
- name: host
    value: "amqp://localhost:5672"
```

Execute the following command (using the Dapr cli) to run the Orders API:

```
dapr run --app-id ordersapi --app-port 5000 --placement-host-address localhost:50000 --components-path ../dapr/components dotnet run
```

Execute the following command (using the Dapr cli) to run the Notification API:

```
dapr run --app-id notificationapi --placement-host-address localhost:50000 --components-path ../dapr/components dotnet run
```

Execute the following command (using the Dapr cli) to run the Faces API:

```
dapr run --app-id facesapi --app-port 6000 --components-path ../dapr/components dotnet run
```

For Faces Web MVC, as it does not need dapr, simply run:

```
dotnet run
```
