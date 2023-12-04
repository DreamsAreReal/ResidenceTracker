# Используйте официальный образ .NET Core для сборки приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./src/ResidenceTracker.Domain/","ResidenceTracker.Domain/"]
COPY ["./src/ResidenceTracker.Frontend/", "ResidenceTracker.Frontend/"]
COPY ["./src/ResidenceTracker.Infrastructure.Abstractions/", "ResidenceTracker.Infrastructure.Abstractions/"]
COPY ["./src/ResidenceTracker.Infrastructure.DataAccess.PostgreSql/", "ResidenceTracker.Infrastructure.DataAccess.PostgreSql/"]
COPY ["./src/ResidenceTracker.UseCases/", "ResidenceTracker.UseCases/"]


RUN dotnet restore "ResidenceTracker.Frontend/ResidenceTracker.Frontend.csproj"
COPY . .
WORKDIR "/src/ResidenceTracker.Frontend"
RUN dotnet build "ResidenceTracker.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ResidenceTracker.Frontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResidenceTracker.Frontend.dll"]