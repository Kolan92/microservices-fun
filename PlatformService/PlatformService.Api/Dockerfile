FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-rc.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.100-rc.2 AS build
WORKDIR /src
COPY . .

RUN dotnet restore "PlatformService.Api.csproj"

RUN dotnet build "PlatformService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlatformService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlatformService.Api.dll"]
