FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/OrderService/OrderService.API/OrderService.API.csproj", "src/Services/OrderService/OrderService.API/"]
COPY ["src/Services/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "src/Services/OrderService/OrderService.Infrastructure/"]
COPY ["src/Services/OrderService/OrderService.Application/OrderService.Application.csproj", "src/Services/OrderService/OrderService.Application/"]
COPY ["src/Services/OrderService/OrderService.Domain/OrderService.Domain.csproj", "src/Services/OrderService/OrderService.Domain/"]
COPY ["src/ServiceDefaults/ServiceDefaults.csproj", "src/ServiceDefaults/"]
COPY ["src/Shared/Shared.BuildingBlocks/Shared.BuildingBlocks.csproj", "src/Shared/Shared.BuildingBlocks/"]
COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]

RUN dotnet restore "src/Services/OrderService/OrderService.API/OrderService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
