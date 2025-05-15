# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY *.sln .
COPY src/Core/EHealthBridgeAPI.Application/EHealthBridgeAPI.Application.csproj src/Core/EHealthBridgeAPI.Application/
COPY src/Core/EHealthBridgeAPI.Domain/EHealthBridgeAPI.Domain.csproj src/Core/EHealthBridgeAPI.Domain/
COPY src/Infrastructure/EHealthBridgeAPI.Infrastructure/EHealthBridgeAPI.Infrastructure.csproj src/Infrastructure/EHealthBridgeAPI.Infrastructure/
COPY src/Infrastructure/EHealthBridgeAPI.Persistence/EHealthBridgeAPI.Persistence.csproj src/Infrastructure/EHealthBridgeAPI.Persistence/
COPY src/Presentation/EHealthBridgeAPI.API/EHealthBridgeAPI.API.csproj src/Presentation/EHealthBridgeAPI.API/
COPY EHealthBridgeApi.UnitTest/EHealthBridgeApi.UnitTest.csproj    EHealthBridgeApi.UnitTest/
#C:\Users\User\Desktop\projects\e-health-bridge-backend\EHealthBridgeApi.UnitTest\EHealthBridgeApi.UnitTest.csproj
#C:\Users\User\Desktop\projects\e-health-bridge-backend\src\Core\EHealthBridgeAPI.Application\EHealthBridgeAPI.Application.csproj
RUN dotnet restore

# Copy everything else and build the project
COPY . .

WORKDIR /src/src/Presentation/EHealthBridgeAPI.API
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EHealthBridgeAPI.API.dll"]