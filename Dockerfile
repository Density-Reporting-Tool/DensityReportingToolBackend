# Use the official .NET 8.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["DensityReportingToolBackend.csproj", "./"]
RUN dotnet restore "DensityReportingToolBackend.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "DensityReportingToolBackend.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DensityReportingToolBackend.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "DensityReportingToolBackend.dll"]
