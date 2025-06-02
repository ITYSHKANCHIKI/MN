# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Копируем решение и проект
COPY *.sln ./
COPY backend/MoralNavigator.API/*.csproj ./backend/MoralNavigator.API/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем весь код и публикуем
COPY . .
WORKDIR /src/backend/MoralNavigator.API
RUN dotnet publish -c Release -o /app/publish

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
# Устанавливаем curl для healthcheck
RUN apt-get update \
 && apt-get install -y curl --no-install-recommends \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000

# Копируем опубликованные файлы
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MoralNavigator.API.dll"]
