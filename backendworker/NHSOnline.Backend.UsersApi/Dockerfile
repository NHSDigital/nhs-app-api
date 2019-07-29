ARG PROJECT_NAME=NHSOnline.Backend.UsersApi
ARG PORT=8088
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
    useradd -m aspnetcore 

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
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV TIMEZONE Europe/London
WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

USER aspnetcore
EXPOSE $PORT
CMD exec dotnet ${PROJECT_NAME}.dll
