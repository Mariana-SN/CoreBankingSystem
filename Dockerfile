FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src/CoreBankingSystem.Domain/CoreBankingSystem.Domain.csproj             src/CoreBankingSystem.Domain/
COPY src/CoreBankingSystem.Application/CoreBankingSystem.Application.csproj   src/CoreBankingSystem.Application/
COPY src/CoreBankingSystem.Infrastructure/CoreBankingSystem.Infrastructure.csproj src/CoreBankingSystem.Infrastructure/
COPY src/CoreBankingSystem.Shared/CoreBankingSystem.Shared.csproj             src/CoreBankingSystem.Shared/
COPY src/CoreBankingSystem.API/CoreBankingSystem.API.csproj                   src/CoreBankingSystem.API/

RUN dotnet restore src/CoreBankingSystem.API/CoreBankingSystem.API.csproj

COPY src/ src/

RUN dotnet publish src/CoreBankingSystem.API/CoreBankingSystem.API.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CoreBankingSystem.API.dll"]