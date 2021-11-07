# Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
#COPY BotEngine/*.csproj ./

# Copy everything else and build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

# Run the app on container startup
# Use your project name for the second parameter
#ENTRYPOINT [ "dotnet", "BotEngine.dll" ]
# for heroku
CMD ASPNETCORE_URLS=http://*:$PORT dotnet BotEngine.dll