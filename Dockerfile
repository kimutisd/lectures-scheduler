FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["LecturesScheduler.WebApi/LecturesScheduler.WebApi.csproj", "LecturesScheduler.WebApi/"]
RUN dotnet restore "LecturesScheduler.WebApi/LecturesScheduler.WebApi.csproj"
COPY . .
WORKDIR "/src/LecturesScheduler.WebApi"
RUN dotnet build "LecturesScheduler.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LecturesScheduler.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LecturesScheduler.WebApi.dll"]