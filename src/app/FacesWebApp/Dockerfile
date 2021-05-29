#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/app/FacesWebApp/FacesWebApp.csproj", "src/app/FacesWebApp/"]
COPY ["src/core/IntegrationEvents/Events/Events.csproj", "src/core/IntegrationEvents/Events/"]
COPY ["src/infrastructure/middleware/dapr/EventBus/EventBus.csproj", "src/infrastructure/middleware/dapr/EventBus/"]
RUN dotnet restore "src/app/FacesWebApp/FacesWebApp.csproj"
COPY . .
WORKDIR "/src/src/app/FacesWebApp"
RUN dotnet build "FacesWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacesWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacesWebApp.dll"]