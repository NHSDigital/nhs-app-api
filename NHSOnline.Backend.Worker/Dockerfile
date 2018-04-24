FROM microsoft/aspnetcore:2.0 AS runtime
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y netcat && \
    rm -rf /var/lib/apt/lists/* && \
    useradd -m aspnetcore && \
    curl -L -o /usr/local/bin/wait-for https://raw.githubusercontent.com/eficode/wait-for/master/wait-for && \
    chmod +rx /usr/local/bin/wait-for

FROM microsoft/aspnetcore-build:2.0 AS build
RUN apt-get update && \
    apt-get upgrade -y && \
    rm -rf /var/lib/apt/lists/*

FROM build as built
WORKDIR /src
COPY *.sln ./
COPY . .
COPY NHSOnline.Backend.Worker/NHSOnline.Backend.Worker.csproj NHSOnline.Backend.Worker/
WORKDIR /src/NHSOnline.Backend.Worker
RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM built AS publish
RUN dotnet publish -c Release -o /app

FROM runtime AS final
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://*:5123"
ENV CORS_AUTHORITY=*
ENV WAIT_FOR_TIMEOUT 60
WORKDIR /app
EXPOSE 5123
COPY --from=publish /app .
RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${REDIS_ODSLOOKUP_CONFIG} -- dotnet NHSOnline.Backend.Worker.dll
