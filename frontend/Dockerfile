FROM node:18.13 as build
WORKDIR /app
COPY yaml-generator-frontend/package*.json ./
RUN npm install --force
COPY yaml-generator-frontend/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/yaml-generator-frontend/browser /usr/share/nginx/html
COPY yaml-generator-frontend/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80 