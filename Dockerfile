# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Graveyard.API/*.csproj Graveyard.API/
RUN dotnet restore Graveyard.API/Graveyard.API.csproj
COPY Graveyard.API/ Graveyard.API/
RUN dotnet publish Graveyard.API/Graveyard.API.csproj -c Release -o /app

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Graveyard.API.dll"]
