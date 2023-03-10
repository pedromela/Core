#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MarketDataService.csproj", "."]
RUN dotnet restore "./MarketDataService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MarketDataService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarketDataService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarketDataService.dll"]