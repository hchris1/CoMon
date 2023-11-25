# Build Angular
FROM node:20-alpine AS frontend

WORKDIR /app
COPY ./angular /app/

RUN npm install --legacy-peer-deps
RUN npm install -g @angular/cli

RUN npm run build

# Build ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
COPY ./ /src
WORKDIR /src

COPY ["aspnet-core/src/CoMon.Web.Host/CoMon.Web.Host.csproj", "aspnet-core/src/CoMon.Web.Host/"]
COPY ["aspnet-core/src/CoMon.Web.Core/CoMon.Web.Core.csproj", "aspnet-core/src/CoMon.Web.Core/"]
COPY ["aspnet-core/src/CoMon.Application/CoMon.Application.csproj", "aspnet-core/src/CoMon.Application/"]
COPY ["aspnet-core/src/CoMon.Core/CoMon.Core.csproj", "aspnet-core/src/CoMon.Core/"]
COPY ["aspnet-core/src/CoMon.EntityFrameworkCore/CoMon.EntityFrameworkCore.csproj", "aspnet-core/src/CoMon.EntityFrameworkCore/"]
RUN dotnet restore "./aspnet-core/src/CoMon.Web.Host/CoMon.Web.Host.csproj"
COPY . .
WORKDIR "/src/aspnet-core/src/CoMon.Web.Host"
RUN dotnet build "./CoMon.Web.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CoMon.Web.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
USER root
RUN apt-get update && apt-get install -y iputils-ping
USER app

COPY --from=publish /app/publish .
COPY --from=frontend /app/dist /app/wwwroot


ENTRYPOINT ["dotnet", "CoMon.Web.Host.dll"]