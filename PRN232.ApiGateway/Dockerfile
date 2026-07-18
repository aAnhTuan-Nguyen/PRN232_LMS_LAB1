FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PRN232.ApiGateway/PRN232.ApiGateway.csproj", "PRN232.ApiGateway/"]
RUN dotnet restore "PRN232.ApiGateway/PRN232.ApiGateway.csproj"
COPY . .
RUN dotnet publish "PRN232.ApiGateway/PRN232.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PRN232.ApiGateway.dll"]
