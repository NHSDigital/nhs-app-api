FROM microsoft/aspnetcore:2.0 AS runtime

FROM microsoft/aspnetcore-build:2.0 AS build
RUN apt-get update
RUN apt-get install -y openjdk-8-jre


FROM build as built
WORKDIR /src
COPY *.sln ./
COPY NHSOnline.Backend.Worker/NHSOnline.Backend.Worker.csproj NHSOnline.Backend.Worker/
RUN dotnet restore
COPY . .
WORKDIR /src/NHSOnline.Backend.Worker
RUN dotnet build -c Release -o /app

FROM built AS publish
RUN dotnet publish -c Release -o /app

FROM runtime
WORKDIR /app
EXPOSE 80
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NHSOnline.Backend.Worker.dll"]
