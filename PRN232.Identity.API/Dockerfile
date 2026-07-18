FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PRN232.Identity.API/PRN232.Identity.API.csproj", "PRN232.Identity.API/"]
COPY ["PRN232.Identity.Services/PRN232.Identity.Services.csproj", "PRN232.Identity.Services/"]
COPY ["PRN232.Identity.Repositories/PRN232.Identity.Repositories.csproj", "PRN232.Identity.Repositories/"]
RUN dotnet restore "PRN232.Identity.API/PRN232.Identity.API.csproj"
COPY . .
RUN dotnet publish "PRN232.Identity.API/PRN232.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PRN232.Identity.API.dll"]
