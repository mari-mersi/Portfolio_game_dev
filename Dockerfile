# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только .csproj для кэширования
COPY *.csproj ./
RUN dotnet restore

# Копируем всё остальное и собираем
COPY . ./
RUN dotnet publish -c Release -o /app

# Этап 2: Запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Render ожидает порт 10000
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Portfolio_game_dev.dll"]