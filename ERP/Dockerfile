# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ERP/*.csproj ./ERP/
RUN dotnet restore

# Copy everything else and build
COPY ERP/. ./ERP/
WORKDIR /app/ERP
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/ERP/out ./
# Enable verbose logging by setting environment variable ASPNETCORE_DETAILEDERRORS and ASPNETCORE_ENVIRONMENT to Development
ENV ASPNETCORE_DETAILEDERRORS=true
ENV ASPNETCORE_ENVIRONMENT=Development
COPY startup.sh /app/startup.sh
RUN chmod +x /app/startup.sh
ENTRYPOINT ["/app/startup.sh"]
