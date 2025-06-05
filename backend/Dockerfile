# -----------------------------
# 1) Build stage: компиляция приложения
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# 1.1) Копируем решение (MN.sln) из корня проекта в /src
COPY ["MN.sln", "./"]

# 1.2) Копируем .csproj проекта в ту же структуру
COPY ["backend/MoralNavigator.API/MoralNavigator.API.csproj", "backend/MoralNavigator.API/"]

# 1.3) Восстанавливаем зависимости в каталоге проекта
WORKDIR /src/backend/MoralNavigator.API
RUN dotnet restore "MoralNavigator.API.csproj"

# 1.4) Копируем весь остальной исходный код из корня проекта
WORKDIR /src
COPY . .

# 1.5) Публикуем проект
WORKDIR /src/backend/MoralNavigator.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# -----------------------------
# 2) Runtime stage: образ с ASP.NET Runtime и publish
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# 2.1) Копируем опубликованные файлы
COPY --from=build /app/publish ./

# 2.2) Пробрасываем порт
EXPOSE 5000

# 2.3) Точка входа
ENTRYPOINT ["dotnet", "MoralNavigator.API.dll"]
