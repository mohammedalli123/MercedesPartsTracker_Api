FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY MercedesPartsTracker.Api/*.csproj ./MercedesPartsTracker.Api/
COPY MercedesPartsTracker.EntityFrameworkModels/*.csproj ./MercedesPartsTracker.EntityFrameworkModels/
COPY MercedesPartsTracker.Services/*.csproj ./MercedesPartsTracker.Services/
COPY MercedesPartsTracker.Services.Tests/*.csproj ./MercedesPartsTracker.Services.Tests/
RUN dotnet restore

COPY . .
WORKDIR /app/MercedesPartsTracker.Api
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/MercedesPartsTracker.Api/out ./

EXPOSE 80

ENTRYPOINT ["dotnet", "MercedesPartsTracker.Api.dll"]
