ARG PROJECT_NAME=NHSOnline.Backend.UserInfoApi
ARG PORT=8091
ARG RUNTIME_IMAGE=nhsapp.azurecr.io/nhsonline-dotnetcore-runtime
ARG SDK_IMAGE=nhsapp.azurecr.io/nhsonline-dotnetcore-build
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
      /var/tmp/* 

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
ARG PROJECT_NAME
ARG PROJECT_PLATFORM
WORKDIR /src
COPY *.sln ./
WORKDIR /src/${PROJECT_NAME}
COPY ${PROJECT_NAME}/${PROJECT_NAME}.csproj ./
RUN dotnet restore  -r ${PROJECT_PLATFORM}

FROM dependencies as built
ARG PROJECT_NAME
ARG PROJECT_PLATFORM
ARG COMMIT_ID
WORKDIR /src
COPY . .
WORKDIR /src/${PROJECT_NAME}
RUN dotnet build -c Release -o /app  -r ${PROJECT_PLATFORM}

FROM built AS publish
ARG PROJECT_PLATFORM
RUN dotnet publish -c Release -o /app -r ${PROJECT_PLATFORM}

FROM runtime AS final
ARG PROJECT_NAME
ARG PORT

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV AUDIT_SINK_TYPE FILE
ENV AUDIT_FILE=audit.txt
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV TIMEZONE Europe/London
ENV USERINFO_MONGO_DATABASE_HOST=
ENV USERINFO_MONGO_DATABASE_PORT=
ENV USERINFO_MONGO_DATABASE_USERNAME=
ENV USERINFO_MONGO_DATABASE_PASSWORD=
ENV USERINFO_MONGO_DATABASE_NAME development
ENV USERINFO_MONGO_DATABASE_COLLECTION info
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
