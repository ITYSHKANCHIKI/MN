FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MoralNavigator.API/MoralNavigator.API.csproj", "MoralNavigator.API/"]
RUN dotnet restore "MoralNavigator.API/MoralNavigator.API.csproj"
COPY . .
WORKDIR "/src/MoralNavigator.API"
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "MoralNavigator.API.dll"]
