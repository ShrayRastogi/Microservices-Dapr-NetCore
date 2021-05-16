#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/api/Ordering/OrdersAPI/OrdersAPI.csproj", "src/api/Ordering/OrdersAPI/"]
COPY ["src/core/IntegrationEvents/EventHandling/OrdersAPI.EventHandling/OrdersAPI.EventHandling.csproj", "src/core/IntegrationEvents/EventHandling/OrdersAPI.EventHandling/"]
COPY ["src/infrastructure/middleware/dapr/EventBus/EventBus.csproj", "src/infrastructure/middleware/dapr/EventBus/"]
COPY ["src/infrastructure/middleware/dapr/StateStore/OrdersAPI.StateStore/OrdersAPI.StateStore.csproj", "src/infrastructure/middleware/dapr/StateStore/OrdersAPI.StateStore/"]
COPY ["src/infrastructure/database/Ordering/OrdersAPI.Database/OrdersAPI.Database.csproj", "src/infrastructure/database/Ordering/OrdersAPI.Database/"]
COPY ["src/core/IntegrationEvents/Events/Events.csproj", "src/core/IntegrationEvents/Events/"]
RUN dotnet restore "src/api/Ordering/OrdersAPI/OrdersAPI.csproj"
COPY . .
WORKDIR "/src/src/api/Ordering/OrdersAPI"
RUN dotnet build "OrdersAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrdersAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersAPI.dll"]