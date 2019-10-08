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
ARG PROJECT_NAME=NHSOnline.Backend.ServiceJourneyRulesApi
ARG PROJECT_PLATFORM
WORKDIR /src
WORKDIR /src/${PROJECT_NAME}
RUN dotnet build --no-restore -c Release -o /app  -r ${PROJECT_PLATFORM}


FROM built AS publish
ARG PROJECT_PLATFORM
RUN dotnet publish --no-restore -c Release -o /app -r ${PROJECT_PLATFORM}

FROM runtime AS final
ARG PROJECT_NAME=NHSOnline.Backend.ServiceJourneyRulesApi
ARG PORT=8086

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
ENV ASPNETCORE_URLS=
ENV AUDIT_SINK_TYPE FILE
ENV SESSION_MONGO_DATABASE_NAME development
ENV GP_INFO_FILE_PATH=
ENV CONFIGURATION_FOLDER_PATH=
ENV RULES_FOLDER_NAME Rules
ENV JOURNEYS_FOLDER_NAME Journeys
ENV OUTPUT_FOLDER_PATH=
ENV INPUT_FOLDER_PATH=
ENV SJR_ARGS=

WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

USER aspnetcore
EXPOSE $PORT
CMD exec dotnet ${PROJECT_NAME}.dll $SJR_ARGS
