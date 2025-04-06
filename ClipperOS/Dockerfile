# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

ARG POSTGRESDBLINK
ARG POSTGRESDBUSER
ARG POSTGRESDBPASS

ENV POSTGRESDBLINK=${POSTGRESDBLINK}
ENV POSTGRESDBUSER=${POSTGRESDBUSER}
ENV POSTGRESDBPASS=${POSTGRESDBPASS}

# Define a porta interna da aplicação
ENV ASPNETCORE_URLS=http://+:8088

WORKDIR /app
EXPOSE 8088
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o .csproj da pasta correta
COPY ClipperOS.csproj ./
RUN dotnet restore "ClipperOS.csproj"

# Copia o restante da aplicação
COPY . ./
RUN dotnet build "ClipperOS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClipperOS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "ClipperOS.dll"]
