FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY NHSOnline.Backend.Worker/NHSOnline.Backend.Worker.csproj NHSOnline.Backend.Worker/
RUN dotnet restore
COPY . .
WORKDIR /src/NHSOnline.Backend.Worker
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NHSOnline.Backend.Worker.dll"]
