FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PRN232.Student.API/PRN232.Student.API.csproj", "PRN232.Student.API/"]
COPY ["PRN232.Student.Services/PRN232.Student.Services.csproj", "PRN232.Student.Services/"]
COPY ["PRN232.Student.Repositories/PRN232.Student.Repositories.csproj", "PRN232.Student.Repositories/"]
COPY ["PRN232.LMS.Contracts/PRN232.LMS.Contracts.csproj", "PRN232.LMS.Contracts/"]
RUN dotnet restore "PRN232.Student.API/PRN232.Student.API.csproj"
COPY . .
RUN dotnet publish "PRN232.Student.API/PRN232.Student.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080 9082
ENTRYPOINT ["dotnet", "PRN232.Student.API.dll"]
