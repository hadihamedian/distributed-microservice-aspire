FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk10.0 AS build
WORKDIR /src

COPY ["src/Services/ProductService/ProductService.API/ProductService.API.csproj", "src/Services/ProductService/ProductService.API/"]
COPY ["src/Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "src/Services/ProductService/ProductService.Infrastructure/"]
COPY ["src/Services/ProductService/ProductService.Application/ProductService.Application.csproj", "src/Services/ProductService/ProductService.Application/"]
COPY ["src/Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "src/Services/ProductService/ProductService.Domain/"]
COPY ["src/ServiceDefaults/ServiceDefaults.csproj", "src/ServiceDefaults/"]
COPY ["src/Shared/Shared.BuildingBlocks/Shared.BuildingBlocks.csproj", "src/Shared/Shared.BuildingBlocks/"]
COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]

RUN dotnet restore "src/Services/ProductService/ProductService.API/ProductService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/ProductService/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]