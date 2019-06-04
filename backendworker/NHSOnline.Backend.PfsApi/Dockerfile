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

FROM build as dependencies
WORKDIR /src
COPY *.sln ./
WORKDIR /src/NHSOnline.Backend.PfsApi
COPY NHSOnline.Backend.PfsApi/NHSOnline.Backend.PfsApi.csproj ./
RUN dotnet restore  -r linux-x64

FROM dependencies as built
WORKDIR /src
COPY . .
WORKDIR /src/NHSOnline.Backend.PfsApi

ARG COMMIT_ID
RUN sed -i "s/.*\"CommitId\":.*/    \"CommitId\": \"$COMMIT_ID\"/" appsettings.json

RUN dotnet build -c Release -o /app  -r linux-x64

FROM built AS publish
RUN dotnet publish -c Release -o /app -r linux-x64
COPY /certs/* /app/certs/

FROM runtime AS final
ENV PORT=
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV CITIZEN_ID_BASE_URL https://auth.ext.signin.nhs.uk/
ENV CITIZEN_ID_CLIENT_ID nhs-online
ENV CITIZEN_ID_CLIENT_SECRET secret
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
ENV ConfigurationSettings__EmisExtendedHttpTimeoutSeconds 120
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
ENV EMIS_CERTIFICATE_PASSWORD=
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
ENV TERMS_CONDITIONS_COSMOS_ENDPOINT_URI=
ENV TERMS_CONDITIONS_COSMOS_AUTH_KEY=
ENV TERMS_CONDITIONS_COSMOS_DATABASE_ID=
ENV TERMS_CONDITIONS_COSMOS_COLLECTION_NAME consent
ENV NDOP_CLAIM_AUDIENCE=https://ndopapp-int1.thunderbird.service.nhs.uk
ENV NDOP_CLAIM_ISSUER=https://www-preview.devuks1.nonlive.nhsapp.service.nhs.uk
ENV STUB_TERMS_AND_CONDITIONS false
ENV IM1CACHE_MONGO_DATABASE_COLLECTION im1cache
ENV ORGAN_DONATION_BASE_URL https://nhsbt-apim-dev01.azure-api.net/uat3/odr/v1/
ENV ORGAN_DONATION_CLIENT_ID ODR-26-NHSAP
ENV ORGAN_DONATION_OCP_APIM_SUBSCRIPTION_KEY 82c9bd8a55df4191873a2b9044c7ecfc
ENV ORGAN_DONATION_INTEGRATION_ENABLED true
ENV ORGAN_DONATION_CERT_PATH=
ENV ORGAN_DONATION_CERT_PASSPHRASE=
ENV ORGAN_DONATION_REFERENCE_DATA_EXPIRY_SECONDS 86400
ENV POSTCODE_LOOKUP_SEARCH_RADIUS_KM=10
ENV GP_LOOKUP_API_RESULTS_LIMIT=20
ENV GP_LOOKUP_API_KEY=
ENV NHS_SEARCH_BASE_URL=https://nhsuk-apim-int-uks.azure-api.net/
ENV VISION_ODS_REMAP_ENABLED false
ENV SERVICE_JOURNEY_RULES_BASE_URL http://servicejourneyrulesapi.local.bitraft.io:8086

WORKDIR /app
EXPOSE $PORT
COPY --from=publish /app .

# Setup trusts for self-signed certificates
COPY NHSOnline.Backend.GpSystems/Suppliers/Tpp/PublicKeys/TppDev.crt /usr/local/share/ca-certificates/TppDev.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Tpp/PublicKeys/TppProd.crt /usr/local/share/ca-certificates/TppProd.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Vision/PublicKeys/vision-test-env.crt /usr/local/share/ca-certificates/vision-test-env.crt
RUN chmod 644 /usr/local/share/ca-certificates/*
RUN update-ca-certificates

RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${REDIS_ODSLOOKUP_CONFIG} -- sudo update-ca-certificates && exec dotnet NHSOnline.Backend.PfsApi.dll
