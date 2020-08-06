# syntax=docker/dockerfile:experimental
FROM nhsapp.azurecr.io/nhsonline-dotnetcore-build:3.1 as dependencies
WORKDIR /src

COPY . .

# fetch nuget caches from volume (nuget does not work with a direct mount)
RUN --mount=type=cache,id=nuget-cache,dst=/tmp/nuget-cache \
    --mount=type=cache,id=nuget-packages,dst=/tmp/nuget-packages \
    mkdir -p /root/.local/share/NuGet \
      && mkdir -p /root/.nuget \
      && mkdir -p /root/.local/share/NuGet/plugins-cache \
      && cp -r /tmp/nuget-cache /root/.local/share/NuGet \
      && cp -r /tmp/nuget-packages /root/.nuget

RUN --mount=type=secret,id=nuget,dst=/root/.nuget/NuGet/NuGet.Config \
    dotnet restore

# update nuget caches volume
RUN --mount=type=cache,id=nuget-cache,dst=/tmp/nuget-cache \
    --mount=type=cache,id=nuget-packages,dst=/tmp/nuget-packages \
    cp -r /root/.local/share/NuGet/v3-cache /tmp/nuget-cache \
      && cp -r /root/.local/share/NuGet/plugins-cache /tmp/nuget-cache \
      && cp -r /root/.nuget/packages /tmp/nuget-packages

FROM dependencies as built
ARG COMMIT_ID
RUN : "${COMMIT_ID:?COMMIT_ID argument needs to be set and non-empty.}"
WORKDIR /src
COPY . .
RUN sed -i "s/.*\"CommitId\":.*/    \"CommitId\": \"$COMMIT_ID\"/" */appsettings.json
RUN dotnet build --no-restore -c Release


FROM built AS published
RUN dotnet publish --no-build -c Release -p:PublishDir=_published_artifacts

FROM nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:3.1 AS final
ARG PROJECT_NAME
RUN : "${PROJECT_NAME:?PROJECT_NAME argument needs to be set and non-empty.}"
ENV PORT=8080
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV TIMEZONE Europe/London


RUN mkdir /app
RUN chown aspnetcore /app
WORKDIR /app
COPY --chown=aspnetcore:aspnetcore --from=published /src/$PROJECT_NAME/_published_artifacts .

# Setup trusts for self-signed certificates if required
COPY SetupCerts.sh .
RUN ./SetupCerts.sh
RUN rm SetupCerts.sh

USER aspnetcore
EXPOSE $PORT
ENV PROJECT_NAME=${PROJECT_NAME}
CMD sudo update-ca-certificates && exec dotnet ${PROJECT_NAME}.dll
