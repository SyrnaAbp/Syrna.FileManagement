#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["host/Syrna.FileManagement.Web.Unified/Syrna.FileManagement.Web.Unified.csproj", "host/Syrna.FileManagement.Web.Unified/"]
COPY ["src/Syrna.FileManagement.Web/Syrna.FileManagement.Web.csproj", "src/Syrna.FileManagement.Web/"]
COPY ["src/Syrna.FileManagement.HttpApi/Syrna.FileManagement.HttpApi.csproj", "src/Syrna.FileManagement.HttpApi/"]
COPY ["src/Syrna.FileManagement.Application.Contracts/Syrna.FileManagement.Application.Contracts.csproj", "src/Syrna.FileManagement.Application.Contracts/"]
COPY ["src/Syrna.FileManagement.Domain.Shared/Syrna.FileManagement.Domain.Shared.csproj", "src/Syrna.FileManagement.Domain.Shared/"]
COPY ["src/Syrna.FileManagement.Application/Syrna.FileManagement.Application.csproj", "src/Syrna.FileManagement.Application/"]
COPY ["src/Syrna.FileManagement.Domain/Syrna.FileManagement.Domain.csproj", "src/Syrna.FileManagement.Domain/"]
COPY ["src/Syrna.FileManagement.EntityFrameworkCore/Syrna.FileManagement.EntityFrameworkCore.csproj", "src/Syrna.FileManagement.EntityFrameworkCore/"]
COPY ["host/Syrna.FileManagement.Host.Shared/Syrna.FileManagement.Host.Shared.csproj", "host/Syrna.FileManagement.Host.Shared/"]
COPY Directory.Build.props .
RUN dotnet restore "host/Syrna.FileManagement.Web.Unified/Syrna.FileManagement.Web.Unified.csproj"
COPY . .
WORKDIR "/src/host/Syrna.FileManagement.Web.Unified"
RUN dotnet build "Syrna.FileManagement.Web.Unified.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Syrna.FileManagement.Web.Unified.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Syrna.FileManagement.Web.Unified.dll"]