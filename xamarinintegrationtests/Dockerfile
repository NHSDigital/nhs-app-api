# syntax=docker/dockerfile:experimental

ARG DOCKER_IMAGE_INTTESTS_BUILD=nhsapp.azurecr.io/nhsonline-dotnetcore-build:3.1
ARG DOCKER_IMAGE_HTTPMOCKS_RUNTIME=nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:3.1

FROM ${DOCKER_IMAGE_INTTESTS_BUILD} as dependencies

RUN apt-get update && \
    apt-get install -y wget gnupg docker.io

WORKDIR /src

COPY *.sln Directory.Build.props nuget.config ./
COPY ./NHSOnline.HttpMocks/NHSOnline.HttpMocks.csproj NHSOnline.HttpMocks/
COPY ./NHSOnline.HttpMocks.Dev/NHSOnline.HttpMocks.Dev.csproj NHSOnline.HttpMocks.Dev/
COPY ./NHSOnline.IntegrationTests/NHSOnline.IntegrationTests.csproj NHSOnline.IntegrationTests/
COPY ./NHSOnline.IntegrationTests.UI/NHSOnline.IntegrationTests.UI.csproj NHSOnline.IntegrationTests.UI/
COPY ./NHSOnline.IntegrationTests.Pages/NHSOnline.IntegrationTests.Pages.csproj NHSOnline.IntegrationTests.Pages/

RUN --mount=type=secret,id=nuget,dst=/root/.nuget/NuGet/NuGet.Config dotnet restore

FROM dependencies as built
WORKDIR /src
COPY . .
RUN dotnet build --no-restore -c Release

FROM built AS published
RUN dotnet publish --no-build -c Release -p:PublishDir=_published_artifacts

FROM ${DOCKER_IMAGE_HTTPMOCKS_RUNTIME} AS httpmocks
WORKDIR /app

COPY --chown=aspnetcore:aspnetcore --from=published /src/NHSOnline.HttpMocks.Dev/_published_artifacts .

ENV PORT=8080
ENV ASPNETCORE_ENVIRONMENT Production
ENV TIMEZONE Europe/London

USER aspnetcore
EXPOSE $PORT
CMD exec dotnet NHSOnline.HttpMocks.Dev.dll

FROM built AS integrationtests
ENV PORT=8080
ENV ASPNETCORE_ENVIRONMENT Production
ENV TIMEZONE Europe/London

EXPOSE $PORT
CMD dotnet test --no-build -c Release -r /src/TestResults --logger "trx;LogFileName=Results.trx" NHSOnline.IntegrationTests/NHSOnline.IntegrationTests.csproj
