FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y netcat sudo && \
    rm -rf /var/lib/apt/lists/* && \
    useradd -m aspnetcore && \
    curl -L -o /usr/local/bin/wait-for https://raw.githubusercontent.com/eficode/wait-for/master/wait-for && \
    chmod +rx /usr/local/bin/wait-for

RUN echo "aspnetcore ALL=(ALL) NOPASSWD: /usr/sbin/update-ca-certificates" >> /etc/sudoers.d/default; \
    chmod 400 /etc/sudoers.d/default

FROM microsoft/dotnet:2.1-sdk AS build
RUN apt-get update && \
    apt-get upgrade -y && \
    rm -rf /var/lib/apt/lists/*

FROM build as built
WORKDIR /src
COPY *.sln ./
COPY . .
COPY NHSOnline.Backend.Worker/NHSOnline.Backend.Worker.csproj NHSOnline.Backend.Worker/
COPY NHSOnline.Backend.Worker/certs/* NHSOnline.Backend.Worker/certs/
WORKDIR /src/NHSOnline.Backend.Worker
RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM built AS publish
RUN dotnet publish -c Release -o /app -r linux-x64
COPY NHSOnline.Backend.Worker/certs/* /app/certs/

FROM runtime AS final
ENV PORT 8082
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://*:8082
ENV CITIZEN_ID_BASE_URL https://auth.ext.signin.nhs.uk/
ENV CITIZEN_ID_CLIENT_ID nhs-online
ENV CITIZEN_ID_CLIENT_SECRET secret
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV EMIS_APPLICATION_ID 16C4B8A9-A6B1-4727-80E3-DA0C755CD6E7
ENV EMIS_BASE_URL https://nhs70apptest.emishealth.com/PFS/
ENV EMIS_VERSION 2.1.0.0
ENV WAIT_FOR_TIMEOUT 60
ENV REDIS_ODSLOOKUP_CONFIG nhsonline.redis.sds:6379
ENV REDIS_SESSION_CONFIG nhsonline.redis.session:6379
ENV ConfigurationSettings__DefaultSessionExpiryMinutes 20
ENV TPP_BASE_URL https://systmonline2.tpp-uk.com:60015/Patient/
ENV TPP_API_VERSION 1
ENV TPP_APPLICATION_NAME NhsApp
ENV TPP_APPLICATION_VERSION 1.0
ENV TPP_APPLICATION_PROVIDER_ID b891fc3b51d5e7c1
ENV TPP_APPLICATION_DEVICE_TYPE NhsApp
ENV VISION_APPLICATION_PROVIDER_ID nhson001
ENV VISION_URI https://52.214.53.197:8443/PatientFacingServices
ENV VISION_CERT_PATH ./certs/vps_nhson001.pfx
ENV VISION_USERNAME vps-nhson001
ENV TIMEZONE Europe/London
ENV SESSION_ENCRYPTION_KEY_FILE DevelopmentSessionEncryptionKey.txt
ENV AUDIT_SINK_TYPE FILE
ENV AUDIT_FILE audit.txt
ENV AUDIT_COSMOS_URI=
ENV AUDIT_COSMOS_KEY=
ENV AUDIT_COSMOS_DATABASE_ID=
ENV AUDIT_COSMOS_COLLECTION_ID audit
ENV CITIZEN_ID_JWT_ISSUER https://auth.ext.signin.nhs.uk
ENV GP_PROVIDER_ENABLED_EMIS true
ENV GP_PROVIDER_ENABLED_TPP true
ENV GP_PROVIDER_ENABLED_VISION true
ENV GP_PROVIDER_ENABLED_MICROTEST true
ENV TERMS_CONDITIONS_COSMOS_ENDPOINT_URI=
ENV TERMS_CONDITIONS_COSMOS_AUTH_KEY=
ENV TERMS_CONDITIONS_COSMOS_DATABASE_ID=
ENV TERMS_CONDITIONS_COSMOS_COLLECTION_NAME consent
ENV NDOP_CLAIM_AUDIENCE=https://ndopapp-int1.thunderbird.service.nhs.uk
ENV NDOP_CLAIM_ISSUER=https://www-preview.devuks1.nonlive.nhsapp.service.nhs.uk
ENV STUB_TERMS_AND_CONDITIONS false

WORKDIR /app
EXPOSE $PORT
COPY --from=publish /app .

# Setup trusts for self-signed certificates
COPY NHSOnline.Backend.Worker/GpSystems/Suppliers/Tpp/PublicKeys/TppCa.crt /usr/local/share/ca-certificates/TppCa.crt
COPY NHSOnline.Backend.Worker/GpSystems/Suppliers/Vision/PublicKeys/vision-test-env.crt /usr/local/share/ca-certificates/vision-test-env.crt
COPY NHSOnline.Backend.Worker/GpSystems/Suppliers/Vision/PublicKeys/visionproxy.crt /usr/local/share/ca-certificates/visionproxy.crt
RUN chmod 644 /usr/local/share/ca-certificates/*
RUN update-ca-certificates

RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${REDIS_ODSLOOKUP_CONFIG} -- sudo update-ca-certificates && dotnet NHSOnline.Backend.Worker.dll
