ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/core/runtime:2.1
ARG SDK_IMAGE=mcr.microsoft.com/dotnet/core/sdk:2.1
ARG PROJECT_PLATFORM=linux-x64

FROM ${RUNTIME_IMAGE} AS runtime
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y netcat sudo && \
    rm -rf \
      /var/cache/debconf/* \
      /var/lib/apt/lists/* \
      /var/log/* \
      /tmp/* \
      /var/tmp/* && \
    useradd -m aspnetcore && \
    curl -L -o /usr/local/bin/wait-for https://raw.githubusercontent.com/eficode/wait-for/master/wait-for && \
    chmod +rx /usr/local/bin/wait-for

RUN echo "aspnetcore ALL=(ALL) NOPASSWD: /usr/sbin/update-ca-certificates" >> /etc/sudoers.d/default; \
    chmod 400 /etc/sudoers.d/default

FROM ${SDK_IMAGE} AS build
RUN apt-get update && \
    apt-get upgrade -y && \
    rm -rf \
      /var/cache/debconf/* \
      /var/lib/apt/lists/* \
      /var/log/* \
      /tmp/* \
      /var/tmp/*

FROM build as dependencies
ARG PROJECT_PLATFORM
WORKDIR /src
COPY ./*.sln ./
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore  -r ${PROJECT_PLATFORM}
COPY . .

FROM dependencies as built
ARG PROJECT_NAME=NHSOnline.Backend.CidApi
ARG PROJECT_PLATFORM
ARG COMMIT_ID
WORKDIR /src
WORKDIR /src/${PROJECT_NAME}
RUN sed -i "s/.*\"CommitId\":.*/    \"CommitId\": \"$COMMIT_ID\"/" appsettings.json
RUN dotnet build --no-restore -c Release -o /app  -r ${PROJECT_PLATFORM}

FROM built AS publish
ARG PROJECT_PLATFORM
RUN dotnet publish --no-restore -c Release -o /app -r ${PROJECT_PLATFORM}
COPY /certs/* /app/certs/

FROM runtime AS final
ARG PROJECT_NAME=NHSOnline.Backend.CidApi
ARG PORT=8084

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV EMIS_APPLICATION_ID 16C4B8A9-A6B1-4727-80E3-DA0C755CD6E7
ENV EMIS_BASE_URL https://nhs70apptest.emishealth.com/PFS/
ENV EMIS_VERSION 2.1.0.0
ENV WAIT_FOR_TIMEOUT 60
ENV REDIS_ODSLOOKUP_CONFIG nhsonline.redis.sds:6379
ENV SESSION_MONGO_DATABASE_HOST=
ENV SESSION_MONGO_DATABASE_PORT=
ENV SESSION_MONGO_DATABASE_USERNAME=
ENV SESSION_MONGO_DATABASE_PASSWORD=
ENV SESSION_MONGO_DATABASE_NAME development
ENV SESSION_MONGO_DATABASE_COLLECTION session
ENV ConfigurationSettings__DefaultSessionExpiryMinutes 10
ENV ConfigurationSettings__EmisExtendedHttpTimeoutSeconds 100
ENV TPP_BASE_URL https://systmonline2.tpp-uk.com:60015/Patient/
ENV TPP_API_VERSION 1
ENV TPP_APPLICATION_NAME NhsApp
ENV TPP_APPLICATION_VERSION 1.0
ENV TPP_APPLICATION_PROVIDER_ID b891fc3b51d5e7c1
ENV TPP_APPLICATION_DEVICE_TYPE NhsApp
ENV VISION_APPLICATION_PROVIDER_ID nhson001
ENV VISION_BASE_URI https://vs002-test.inps.co.uk/
ENV VISION_PFS_PATH PatientFacingServices
ENV VISION_LINKAGE_PATH serverApi/v1/
ENV VISION_CERT_PATH ./certs/vps_nhson001.pfx
ENV VISION_USERNAME vps-nhson001
ENV VISION_SENDER_USERNAME Username123123asdas
ENV VISION_SENDER_USERFULLNAME Name
ENV VISION_SENDER_USERIDENTITY 65489761321654
ENV VISION_SENDER_USERROLE ROSU
ENV EMIS_CERTIFICATE_PATH=
ENV TIMEZONE Europe/London
ENV SESSION_ENCRYPTION_KEY_FILE DevelopmentSessionEncryptionKey.txt
ENV MICROTEST_BASE_URL https://nhsapp.microtest.co.uk/v1/
ENV MICROTEST_CERT_PATH ./certs/microtest-dev.pfx
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
ENV STUB_TERMS_AND_CONDITIONS false
ENV IM1CACHE_MONGO_DATABASE_COLLECTION im1cache
ENV VISION_ODS_REMAP_ENABLED false

WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

# Setup trusts for self-signed certificates
COPY NHSOnline.Backend.GpSystems/Suppliers/Tpp/PublicKeys/TppDev.crt /usr/local/share/ca-certificates/TppDev.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Tpp/PublicKeys/TppProd.crt /usr/local/share/ca-certificates/TppProd.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Vision/PublicKeys/vision-test-env.crt /usr/local/share/ca-certificates/vision-test-env.crt
RUN chmod 644 /usr/local/share/ca-certificates/*
RUN update-ca-certificates

USER aspnetcore
EXPOSE $PORT
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${REDIS_ODSLOOKUP_CONFIG} -- sudo update-ca-certificates && exec dotnet ${PROJECT_NAME}.dll
