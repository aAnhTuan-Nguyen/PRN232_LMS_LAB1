FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PRN232.Course.API/PRN232.Course.API.csproj", "PRN232.Course.API/"]
COPY ["PRN232.Course.Services/PRN232.Course.Services.csproj", "PRN232.Course.Services/"]
COPY ["PRN232.Course.Repositories/PRN232.Course.Repositories.csproj", "PRN232.Course.Repositories/"]
COPY ["PRN232.LMS.Contracts/PRN232.LMS.Contracts.csproj", "PRN232.LMS.Contracts/"]
RUN dotnet restore "PRN232.Course.API/PRN232.Course.API.csproj"
COPY . .
RUN dotnet publish "PRN232.Course.API/PRN232.Course.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080 9083
ENTRYPOINT ["dotnet", "PRN232.Course.API.dll"]
