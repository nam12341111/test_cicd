# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["server.sln", "./"]

# Copy project files
COPY ["PTJ.API/PTJ.API.csproj", "PTJ.API/"]
COPY ["PTJ.Application/PTJ.Application.csproj", "PTJ.Application/"]
COPY ["PTJ.Domain/PTJ.Domain.csproj", "PTJ.Domain/"]
COPY ["PTJ.Infrastructure/PTJ.Infrastructure.csproj", "PTJ.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "server.sln"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/PTJ.API"
RUN dotnet build "PTJ.API.csproj" -c Release -o /app/build

# Stage 2: Publish stage
FROM build AS publish
RUN dotnet publish "PTJ.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create directory for file uploads
RUN mkdir -p /app/Uploads

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "PTJ.API.dll"]
