#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY Service.WebApi/Service.WebApi.csproj Service.WebApi/
COPY Service.Core/Service.Core.csproj Service.Core/
COPY Service.Domain/Service.Domain.csproj Service.Domain/
COPY Service.Infrastructure/Service.Infrastructure.csproj Service.Infrastructure/
RUN dotnet restore "Service.WebApi/Service.WebApi.csproj"
COPY . .
WORKDIR "/src/Service.WebApi"
# RUN dotnet build "Service.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.WebApi.dll"]
