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
ARG PROJECT_NAME=NHSOnline.Backend.LoggerApi
ARG PROJECT_PLATFORM
ARG COMMIT_ID
WORKDIR /src
WORKDIR /src/${PROJECT_NAME}
RUN sed -i "s/.*\"CommitId\":.*/    \"CommitId\": \"$COMMIT_ID\"/" appsettings.json
RUN dotnet build --no-restore -c Release -o /app  -r ${PROJECT_PLATFORM}

FROM built AS publish
ARG PROJECT_PLATFORM
RUN dotnet publish --no-restore -c Release -o /app -r ${PROJECT_PLATFORM}

FROM runtime AS final
ARG PROJECT_NAME=NHSOnline.Backend.LoggerApi
ARG PORT=8090

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV TIMEZONE Europe/London
ENV CLIENT_LOGGER_LEVEL error

WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

USER aspnetcore
EXPOSE $PORT
CMD exec dotnet ${PROJECT_NAME}.dll