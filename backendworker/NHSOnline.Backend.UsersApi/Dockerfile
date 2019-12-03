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
ARG PROJECT_NAME=NHSOnline.Backend.UsersApi
ARG PROJECT_PLATFORM
ARG COMMIT_ID
WORKDIR /src
WORKDIR /src/${PROJECT_NAME}
RUN dotnet build --no-restore -c Release -o /app  -r ${PROJECT_PLATFORM}

FROM built AS publish
ARG PROJECT_PLATFORM
RUN dotnet publish --no-restore -c Release -o /app -r ${PROJECT_PLATFORM}

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-runtime AS final
ARG PROJECT_NAME=NHSOnline.Backend.UsersApi
ARG PORT=8083

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV AUDIT_SINK_TYPE FILE
ENV AUDIT_FILE=audit.txt
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV TIMEZONE Europe/London
ENV USERS_MONGO_DATABASE_HOST=
ENV USERS_MONGO_DATABASE_PORT=
ENV USERS_MONGO_DATABASE_USERNAME=
ENV USERS_MONGO_DATABASE_PASSWORD=
ENV USERS_MONGO_DATABASE_NAME development
ENV USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION devices
ENV AZURE_NOTIFICATION_HUB_PATH=nhsapp-nonlive
ENV AZURE_NOTIFICATION_HUB_CONNECTION_STRING=Endpoint=sb://nhsapp-nonlive.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=
ENV AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY=
ENV CITIZEN_ID_BASE_URL https://auth.ext.signin.nhs.uk/
ENV CITIZEN_ID_CLIENT_ID nhs-online
ENV CITIZEN_ID_CLIENT_SECRET secret
ENV CITIZEN_ID_JWT_ISSUER https://auth.ext.signin.nhs.uk

WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

USER aspnetcore
EXPOSE $PORT
CMD sudo update-ca-certificates && exec dotnet ${PROJECT_NAME}.dll
