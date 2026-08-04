# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["BusinessOperations.API.csproj", "./"]
RUN dotnet restore "BusinessOperations.API.csproj"

# Copy remaining source code and publish
COPY . .
RUN dotnet publish "BusinessOperations.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5000
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
RUN mkdir -p /app/Logs /app/Backups /app/wwwroot/uploads \
    && chown -R app:app /app/Logs /app/Backups /app/wwwroot/uploads
USER app
ENTRYPOINT ["dotnet", "BusinessOperations.API.dll"]
