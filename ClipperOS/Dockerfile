FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

ARG POSTGRESDBLINK
ARG POSTGRESDBUSER
ARG POSTGRESDBPASS

ENV POSTGRESDBLINK=POSTGRESDBLINK
ENV POSTGRESDBUSER=POSTGRESDBUSER
ENV POSTGRESDBPASS=POSTGRESDBPASS


USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ClipperOS/ClipperOS/ClipperOS.csproj", "ClipperOS/"]
RUN dotnet restore "ClipperOS/ClipperOS/ClipperOS.csproj"
COPY . .
WORKDIR "/src/ClipperOS"
RUN dotnet build "ClipperOS.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClipperOS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClipperOS.dll"]
