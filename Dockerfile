FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so layer cache is reused when only source changes
COPY LaerdalImplementation.sln .
COPY src/LaerdalImplementation.Domain/LaerdalImplementation.Domain.csproj             src/LaerdalImplementation.Domain/
COPY src/LaerdalImplementation.Application/LaerdalImplementation.Application.csproj   src/LaerdalImplementation.Application/
COPY src/LaerdalImplementation.Infrastructure/LaerdalImplementation.Infrastructure.csproj src/LaerdalImplementation.Infrastructure/
COPY src/LaerdalImplementation.Api/LaerdalImplementation.Api.csproj                   src/LaerdalImplementation.Api/
COPY tests/LaerdalImplementation.Tests/LaerdalImplementation.Tests.csproj             tests/LaerdalImplementation.Tests/

RUN dotnet restore src/LaerdalImplementation.Api/LaerdalImplementation.Api.csproj

# Copy all source
COPY src/ src/

RUN dotnet publish src/LaerdalImplementation.Api/LaerdalImplementation.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LaerdalImplementation.Api.dll"]
