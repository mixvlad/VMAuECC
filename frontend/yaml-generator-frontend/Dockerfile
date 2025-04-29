# Этап сборки
FROM node:18 as build
WORKDIR /app

# Принимаем аргумент для определения окружения
ARG ENV=prod

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Этап продакшена
FROM nginx:alpine
COPY --from=build /app/dist/yaml-generator-frontend /usr/share/nginx/html

# Копируем соответствующий конфиг в зависимости от окружения
COPY nginx/nginx.${ENV}.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]