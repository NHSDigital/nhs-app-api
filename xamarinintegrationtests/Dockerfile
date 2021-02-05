# syntax=docker/dockerfile:experimental

ARG BASE_IMAGE
FROM ${BASE_IMAGE} as dependencies

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

FROM built AS integrationtests
ENV PORT=8080
ENV ASPNETCORE_ENVIRONMENT Production
ENV TIMEZONE Europe/London

EXPOSE $PORT
CMD dotnet test --no-build -c Release -r /src/TestResults --logger "trx;LogFileName=Results.trx" NHSOnline.IntegrationTests/NHSOnline.IntegrationTests.csproj
