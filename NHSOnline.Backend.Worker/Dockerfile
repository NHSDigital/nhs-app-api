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
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://*:8082
ENV CITIZEN_ID_BASE_URL https://keycloak.dev1.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/
ENV CITIZEN_ID_CLIENT_ID nhs-online-poc
ENV CITIZEN_ID_CLIENT_SECRET 872b46c9-0afe-4037-8958-e7bbc209a379
ENV CORS_AUTHORITY https://www-preview.nonliveeuw.bitraft.io
ENV EMIS_APPLICATION_ID D66BA979-60D2-49AA-BE82-AEC06356E41F
ENV EMIS_BASE_URL http://nhsonline.stubs:8080/emis/
ENV EMIS_VERSION 2.1.0.0
ENV NHS_WEB_APP_BASE_URL https://www-preview.nonliveeuw.bitraft.io/
ENV SESSION_EXPIRY_MINUTES 20
ENV WAIT_FOR_TIMEOUT 60
ENV REDIS_ODSLOOKUP_CONFIG nhsonline.redis.sds:6379
ENV REDIS_SESSION_CONFIG nhsonline.redis.session:6379

WORKDIR /app
EXPOSE 8082
COPY --from=publish /app .
RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${REDIS_ODSLOOKUP_CONFIG} -- dotnet NHSOnline.Backend.Worker.dll
