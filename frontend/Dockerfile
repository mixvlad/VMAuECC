# Build stage
FROM node:22-alpine AS build
WORKDIR /app
COPY yaml-generator-frontend/package*.json ./
RUN npm install
COPY yaml-generator-frontend/ .
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/dist/yaml-generator-frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
