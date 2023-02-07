FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LabaPostgreSQL.csproj", "LabaPostgreSQL/"]
RUN dotnet restore "LabaPostgreSQL/LabaPostgreSQL.csproj"

WORKDIR "/src/LabaPostgreSQL"
COPY . .
RUN dotnet build "LabaPostgreSQL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LabaPostgreSQL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LabaPostgreSQL.dll"]
