# STAGE 1: Build (Standard SDK)
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src

# Optimize Restore: Copy only project files first to leverage Docker cache
COPY ["ERP_Backend/ERP.csproj", "ERP_Backend/"]
COPY ERP_Backend/ ./ERP_Backend/
RUN dotnet restore "ERP/ERP.csproj"

# Copy remaining source and build
WORKDIR "/src/ERP_Backend"

RUN dotnet publish "ERP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Runtime (Chiseled - Ultra Small & Secure)
# Using 'aspnet:9.0-jammy-chiseled' for the smallest footprint
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy-chiseled AS final
WORKDIR /app
COPY --from=build /app/publish .

# ERP Finance Config
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production 
# ^ Keep Development only for local non-K8s debugging

# Chiseled images run as a non-root user (ID 1654) by default for security
USER app
ENTRYPOINT ["dotnet", "ERP.dll"]