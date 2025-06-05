#
# ─────────────────────────────────────────────────────────────────────────────
# 1) Этап сборки (Build) на базе Node.js + Alpine
# ─────────────────────────────────────────────────────────────────────────────
FROM node:18-alpine AS build

# 1.1) Устанавливаем рабочую директорию
WORKDIR /app

# 1.2) Копируем package.json и package-lock.json (если у вас используется yarn.lock,
#      его можно также скопировать сюда в дополнение — но я предполагаю, что у вас package-lock.json)
COPY package.json package-lock.json ./

# 1.3) Устанавливаем root-пакеты (включая devDependencies, чтобы vite попал в node_modules/.bin)
RUN npm install

# 1.4) Копируем в контейнер весь исходный код фронтенда (включая vite.config.js, src/, public/ и т.д.)
COPY . .

# 1.5) Запускаем сборку Vite; результат положится в ./dist
RUN npm run build


#
# ─────────────────────────────────────────────────────────────────────────────
# 2) Этап рантайма (Runtime): копируем готовый бандл в образ Nginx и настраиваем
# ─────────────────────────────────────────────────────────────────────────────
FROM nginx:alpine

# 2.1) Удаляем дефолтный конфиг Nginx (он нам не нужен)
RUN rm /etc/nginx/conf.d/default.conf

# 2.2) Копируем наш кастомный nginx.conf (см. следующий раздел)
COPY nginx.conf /etc/nginx/conf.d/

# 2.3) Копируем сгенерированную папку dist из предыдущего слоя build в статический каталог Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# 2.4) Открываем порт 80
EXPOSE 80

# 2.5) Запускаем nginx в foreground
CMD ["nginx", "-g", "daemon off;"]
