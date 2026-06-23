FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# کپی کردن فایل‌های پروژه جهت ریستور پکیج‌ها
COPY ["src/Services/RecommendationService/RecommendationService.API/RecommendationService.API.csproj", "src/Services/RecommendationService/RecommendationService.API/"]
COPY ["src/Services/RecommendationService/RecommendationService.Infrastructure/RecommendationService.Infrastructure.csproj", "src/Services/RecommendationService/RecommendationService.Infrastructure/"]
COPY ["src/Services/RecommendationService/RecommendationService.Application/RecommendationService.Application.csproj", "src/Services/RecommendationService/RecommendationService.Application/"]
COPY ["src/ServiceDefaults/ServiceDefaults.csproj", "src/ServiceDefaults/"]
COPY ["src/Shared/Shared.BuildingBlocks/Shared.BuildingBlocks.csproj", "src/Shared/Shared.BuildingBlocks/"]
COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]

RUN dotnet restore "src/Services/RecommendationService/RecommendationService.API/RecommendationService.API.csproj"

# کپی کردن کل سورس کد و بیلد پروژه
COPY . .
WORKDIR "/src/src/Services/RecommendationService/RecommendationService.API"
RUN dotnet build "RecommendationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecommendationService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecommendationService.API.dll"]