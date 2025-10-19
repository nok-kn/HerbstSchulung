# syntax=docker/dockerfile:1

# Mehrstufiger Build für kleinere Images
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# ASP.NET Core verwendet standardmäßig Kestrel

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HerbstSchulung.WebApi/HerbstSchulung.WebApi.csproj", "HerbstSchulung.WebApi/"]
COPY ["HerbstSchulung.Hosting.Services/HerbstSchulung.Hosting.Services.csproj", "HerbstSchulung.Hosting.Services/"]
COPY ["HerbstSchulung.Hosting.Abstractions/HerbstSchulung.Hosting.Abstractions.csproj", "HerbstSchulung.Hosting.Abstractions/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "HerbstSchulung.WebApi/HerbstSchulung.WebApi.csproj"
COPY . .
WORKDIR "/src/HerbstSchulung.WebApi"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Standard-Port für ASP.NET Core in Container
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HerbstSchulung.WebApi.dll"]
