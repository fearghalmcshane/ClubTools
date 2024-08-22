#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ClubTools.API/ClubTools.Api.csproj", "src/ClubTools.API/"]
RUN dotnet restore "src/ClubTools.API/ClubTools.Api.csproj"
COPY . .
WORKDIR "/src/src/ClubTools.API"
RUN dotnet build "ClubTools.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClubTools.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClubTools.API.dll"]