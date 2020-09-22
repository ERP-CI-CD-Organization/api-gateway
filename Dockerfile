# 3.1 (238MB) vs 3.1-alpine (138MB)
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# mcr.microsoft.com/dotnet/core/aspnet:3.1 | ASP.NET Core, with runtime only and ASP.NET Core optimizations, on Linux and Windows (multi-arch)
# mcr.microsoft.com/dotnet/core/sdk:3.1    |.NET Core, with SDKs included, on Linux and Windows (multi-arch) | Size after build:238MB
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore APIGateway.csproj

COPY . .


RUN dotnet publish -c release -o /app --no-restore

# final stage/image
# 3.1
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "APIGateway.dll"]


