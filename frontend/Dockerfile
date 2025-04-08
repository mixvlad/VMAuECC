# Use a newer Node.js version that meets Angular CLI requirements
FROM node:18.19.1 as build

WORKDIR /app

COPY yaml-generator-frontend/package*.json ./
RUN npm install --force
COPY yaml-generator-frontend/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/yaml-generator-frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
