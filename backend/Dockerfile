FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["YamlGenerator.API/YamlGenerator.API.csproj", "YamlGenerator.API/"]
COPY ["YamlGenerator.Core/YamlGenerator.Core.csproj", "YamlGenerator.Core/"]
COPY ["YamlGenerator.Tests/YamlGenerator.Tests.csproj", "YamlGenerator.Tests/"]
RUN dotnet restore "YamlGenerator.API/YamlGenerator.API.csproj"

# Копируем весь исходный код
COPY . .

# Собираем проект
WORKDIR "/src/YamlGenerator.API"
RUN dotnet build "YamlGenerator.API.csproj" -c Release -o /app/build

# Публикуем проект
FROM build AS publish
RUN dotnet publish "YamlGenerator.API.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YamlGenerator.API.dll"]
