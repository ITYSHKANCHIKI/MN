# Stage 1: сборка на Node.js
FROM node:18-alpine AS build
WORKDIR /app

# Копируем только package.json (lock-файла нет)
COPY package.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем исходники и билдим
COPY . .
RUN npm run build

# Stage 2: Nginx для отдачи статики + прокси
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Копируем собранный билд
COPY --from=build /app/dist .

# Копируем наш конфиг вместо дефолтного
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Nginx сам запустится
