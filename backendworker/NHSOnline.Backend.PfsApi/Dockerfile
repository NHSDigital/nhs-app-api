ARG PROJECT_PLATFORM=linux-x64

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-build as dependencies
ARG PROJECT_PLATFORM
WORKDIR /src
COPY ./*.sln ./
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore  -r ${PROJECT_PLATFORM}
COPY . .

FROM dependencies as built
ARG PROJECT_NAME=NHSOnline.Backend.PfsApi
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

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-runtime AS final
ARG PROJECT_NAME=NHSOnline.Backend.PfsApi
ARG PORT=8082

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
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
ENV EMIS_CERTIFICATE_PASSWORD=
ENV TIMEZONE Europe/London
ENV SESSION_ENCRYPTION_KEY_FILE DevelopmentSessionEncryptionKey.txt
ENV MICROTEST_BASE_URL https://nhsapp-test.microtest.co.uk/v1/
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
ENV PHARMACY_API_RESULT_LIMIT=10
ENV VISION_ODS_REMAP_ENABLED false
ENV SERVICE_JOURNEY_RULES_BASE_URL http://servicejourneyrulesapi.local.bitraft.io:8086
ENV PDS_QUERY_FROM_ADDRESS 10.0.255.22
ENV PDS_QUERY_TO https://192.168.128.11/syncservice-pds/pds
ENV SPINE_PDS_URL https://40.120.51.179:80/
ENV DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS 500
ENV NOMINATED_PHARMACY_ENABLED true
ENV PROXY_ACCESS_ENABLED false
ENV GP_PRACTICE_ODS_CODE_FOR_EPS_ENABLED_CHECK_OVERRIDE B83054
ENV SPINE_LDAP_LOOKUP_ENABLED true
ENV SPINE_LDAP_HOST=40.120.51.179
ENV SPINE_LDAP_PORT=443
ENV SPINE_LDAP_LOGIN_DN="ou=services, o=nhs"
ENV SPINE_CERT_PATH=./certs/spine.pfx
ENV SPINE_CERT_PASSWORD=
ENV NHS_APP_PARTY_ID_FOR_SPINE=E87021-820463

WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

# Setup trusts for self-signed certificates
COPY NHSOnline.Backend.GpSystems/Suppliers/Tpp/PublicKeys/TppDev.crt /usr/local/share/ca-certificates/TppDev.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Tpp/PublicKeys/TppProd.crt /usr/local/share/ca-certificates/TppProd.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Vision/PublicKeys/vision-test-env.crt /usr/local/share/ca-certificates/vision-test-env.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Spine/PublicKeys/spine_ldap_search_test_env.crt /usr/local/share/ca-certificates/spine_ldap_search_test_env.crt
COPY NHSOnline.Backend.GpSystems/Suppliers/Spine/PublicKeys/queue-staging.nhsapp.service.nhs.uk.crt /usr/local/share/ca-certificates/queue-staging.nhsapp.service.nhs.uk.crt
RUN chmod 644 /usr/local/share/ca-certificates/*
RUN update-ca-certificates

USER aspnetcore
EXPOSE $PORT
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${REDIS_ODSLOOKUP_CONFIG} -- sudo update-ca-certificates && exec dotnet ${PROJECT_NAME}.dll
