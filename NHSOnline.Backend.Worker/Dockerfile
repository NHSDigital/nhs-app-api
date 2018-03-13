FROM microsoft/aspnetcore:2.0 AS runtime
RUN apt-get update && \
    apt-get upgrade -y && \
    rm -rf /var/lib/apt/lists/* && \
    adduser --disabled-password aspnetcore

FROM microsoft/aspnetcore-build:2.0 AS build
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y openjdk-8-jre && \
    rm -rf /var/lib/apt/lists/*

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

FROM runtime AS final
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://*:5123"
WORKDIR /app
EXPOSE 5123
COPY --from=publish /app .
RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD dotnet NHSOnline.Backend.Worker.dll
