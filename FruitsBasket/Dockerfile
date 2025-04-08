FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FruitsBasket.Api/FruitsBasket.Api.csproj", "FruitsBasket.Api/"]
COPY ["FruitsBasket.Data/FruitsBasket.Data.csproj", "FruitsBasket.Data/"]
COPY ["FruitsBasket.Model/FruitsBasket.Model.csproj", "FruitsBasket.Model/"]
COPY ["FruitsBasket.Orchestrator/FruitsBasket.Orchestrator.csproj", "FruitsBasket.Orchestrator/"]
RUN dotnet restore "FruitsBasket.Api/FruitsBasket.Api.csproj"
COPY . .
WORKDIR "/src/FruitsBasket.Api"
RUN dotnet build "FruitsBasket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FruitsBasket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FruitsBasket.Api.dll"]
