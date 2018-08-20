FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY BookWebApi/BookWebApi.csproj BookWebApi/
RUN dotnet restore BookWebApi/BookWebApi.csproj
COPY . .
WORKDIR /src/BookWebApi
RUN dotnet build BookWebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish BookWebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BookWebApi.dll"]
