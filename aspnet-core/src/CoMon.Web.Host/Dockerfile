FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["src/CoMon.Web.Host/CoMon.Web.Host.csproj", "src/CoMon.Web.Host/"]
COPY ["src/CoMon.Web.Core/CoMon.Web.Core.csproj", "src/CoMon.Web.Core/"]
COPY ["src/CoMon.Application/CoMon.Application.csproj", "src/CoMon.Application/"]
COPY ["src/CoMon.Core/CoMon.Core.csproj", "src/CoMon.Core/"]
COPY ["src/CoMon.EntityFrameworkCore/CoMon.EntityFrameworkCore.csproj", "src/CoMon.EntityFrameworkCore/"]
WORKDIR "/src/src/CoMon.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/CoMon.Web.Host", "src/CoMon.Web.Host"]
COPY ["src/CoMon.Web.Core", "src/CoMon.Web.Core"]
COPY ["src/CoMon.Application", "src/CoMon.Application"]
COPY ["src/CoMon.Core", "src/CoMon.Core"]
COPY ["src/CoMon.EntityFrameworkCore", "src/CoMon.EntityFrameworkCore"]
WORKDIR "/src/src/CoMon.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "CoMon.Web.Host.dll"]
