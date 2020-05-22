# syntax=docker/dockerfile:experimental
FROM nhsapp.azurecr.io/nhsonline-dotnetcore-build:3.1 as dependencies
WORKDIR /src

COPY Directory.Build.props \
  nuget.config \
  NHSOnline.Backend.Auditing.UnitTests/NHSOnline.Backend.Auditing.UnitTests.csproj \
  NHSOnline.Backend.Auditing/NHSOnline.Backend.Auditing.csproj \
  NHSOnline.Backend.Auth.UnitTests/NHSOnline.Backend.Auth.UnitTests.csproj \
  NHSOnline.Backend.Auth/NHSOnline.Backend.Auth.csproj \
  NHSOnline.Backend.CidApi.UnitTests/NHSOnline.Backend.CidApi.UnitTests.csproj \
  NHSOnline.Backend.CidApi/NHSOnline.Backend.CidApi.csproj \
  NHSOnline.Backend.GpSystems.UnitTests/NHSOnline.Backend.GpSystems.UnitTests.csproj \
  NHSOnline.Backend.GpSystems/NHSOnline.Backend.GpSystems.csproj \
  NHSOnline.Backend.LoggerApi.UnitTests/NHSOnline.Backend.LoggerApi.UnitTests.csproj \
  NHSOnline.Backend.LoggerApi/NHSOnline.Backend.LoggerApi.csproj \
  NHSOnline.Backend.MessagesApi.UnitTests/NHSOnline.Backend.MessagesApi.UnitTests.csproj \
  NHSOnline.Backend.MessagesApi/NHSOnline.Backend.MessagesApi.csproj \
  NHSOnline.Backend.Metrics.UnitTests/NHSOnline.Backend.Metrics.UnitTests.csproj \
  NHSOnline.Backend.Metrics/NHSOnline.Backend.Metrics.csproj \
  NHSOnline.Backend.NominatedPharmacy.UnitTests/NHSOnline.Backend.NominatedPharmacy.UnitTests.csproj \
  NHSOnline.Backend.NominatedPharmacy/NHSOnline.Backend.NominatedPharmacy.csproj \
  NHSOnline.Backend.PfsApi.UnitTests/NHSOnline.Backend.PfsApi.UnitTests.csproj \
  NHSOnline.Backend.PfsApi/NHSOnline.Backend.PfsApi.csproj \
  NHSOnline.Backend.ServiceJourneyRules.Common.UnitTests/NHSOnline.Backend.ServiceJourneyRules.Common.UnitTests.csproj \
  NHSOnline.Backend.ServiceJourneyRules.Common/NHSOnline.Backend.ServiceJourneyRules.Common.csproj \
  NHSOnline.Backend.ServiceJourneyRulesApi.Models/NHSOnline.Backend.ServiceJourneyRulesApi.Models.csproj \
  NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests/NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.csproj \
  NHSOnline.Backend.ServiceJourneyRulesApi/NHSOnline.Backend.ServiceJourneyRulesApi.csproj \
  NHSOnline.Backend.Support.UnitTests/NHSOnline.Backend.Support.UnitTests.csproj \
  NHSOnline.Backend.Support/NHSOnline.Backend.Support.csproj \
  NHSOnline.Backend.UserInfoApi.UnitTests/NHSOnline.Backend.UserInfoApi.UnitTests.csproj \
  NHSOnline.Backend.UserInfoApi/NHSOnline.Backend.UserInfoApi.csproj \
  NHSOnline.Backend.UsersApi.UnitTests/NHSOnline.Backend.UsersApi.UnitTests.csproj \
  NHSOnline.Backend.UsersApi/NHSOnline.Backend.UsersApi.csproj \
  NHSOnline.Backend.Worker.sln \
  UnitTestHelper/UnitTestHelper.csproj \
  ./

RUN for file in *.csproj; do mkdir "${file%.*}" && mv "$file" "${file%.*}"; done
RUN --mount=type=secret,id=nuget,dst=/root/.nuget/NuGet/NuGet.Config dotnet restore

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
