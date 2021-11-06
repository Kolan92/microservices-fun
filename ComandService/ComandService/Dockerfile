FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ComandService/ComandService.csproj", "ComandService/"]
RUN dotnet restore "ComandService/ComandService.csproj"
COPY . .
WORKDIR "/src/ComandService"
RUN dotnet build "ComandService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ComandService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ComandService.dll"]
