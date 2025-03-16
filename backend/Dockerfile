FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["YamlGenerator.API/YamlGenerator.API.csproj", "YamlGenerator.API/"]
COPY ["YamlGenerator.Core/YamlGenerator.Core.csproj", "YamlGenerator.Core/"]
RUN dotnet restore "YamlGenerator.API/YamlGenerator.API.csproj"

COPY . .
RUN dotnet build "YamlGenerator.API/YamlGenerator.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YamlGenerator.API/YamlGenerator.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8283
ENTRYPOINT ["dotnet", "YamlGenerator.API.dll"] 