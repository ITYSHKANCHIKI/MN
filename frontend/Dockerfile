# 1) Сборка SPA с помощью Node.js
FROM node:18-alpine AS build
WORKDIR /app

COPY package.json yarn.lock* package-lock.json* ./
RUN npm install

COPY . .
RUN npm run build

# 2) Финальный образ на базе Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Копируем готовый билд
COPY --from=build /app/dist ./

# Копируем конфигурацию Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
