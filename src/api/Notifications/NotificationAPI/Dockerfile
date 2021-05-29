#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/api/Notifications/NotificationAPI/NotificationAPI.csproj", "src/api/Notifications/NotificationAPI/"]
COPY ["src/core/IntegrationEvents/EventHandling/NotificationAPI.EventHandling/NotificationAPI.EventHandling.csproj", "src/core/IntegrationEvents/EventHandling/NotificationAPI.EventHandling/"]
COPY ["src/infrastructure/middleware/dapr/StateStore/StateStore.csproj", "src/infrastructure/middleware/dapr/StateStore/"]
COPY ["src/core/IntegrationEvents/Events/Events.csproj", "src/core/IntegrationEvents/Events/"]
COPY ["src/infrastructure/middleware/dapr/EventBus/EventBus.csproj", "src/infrastructure/middleware/dapr/EventBus/"]
RUN dotnet restore "src/api/Notifications/NotificationAPI/NotificationAPI.csproj"
COPY . .
WORKDIR "/src/src/api/Notifications/NotificationAPI"
RUN dotnet build "NotificationAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationAPI.dll"]