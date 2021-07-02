ARG SJR_IMAGE=nhsonline.backend.servicejourneyrulesapi

FROM ${SJR_IMAGE} AS validated
WORKDIR /app
COPY --chown=aspnetcore:aspnetcore gpinfo.csv ./configurations/gpinfo.csv
COPY --chown=aspnetcore:aspnetcore Journeys/ ./configurations/Journeys/
COPY --chown=aspnetcore:aspnetcore Rules/ ./configurations/Rules/
COPY --chown=aspnetcore:aspnetcore Globals/ ./configurations/Globals/

ENV GP_INFO_FILE_PATH ./configurations/gpinfo.csv
ENV CONFIGURATION_FOLDER_PATH ./configurations
ENV RULES_FOLDER_NAME Rules
ENV JOURNEYS_FOLDER_NAME Journeys
ENV GLOBALS_FOLDER_NAME Globals
ENV OUTPUT_FOLDER_PATH ./configurations/output
RUN dotnet NHSOnline.Backend.ServiceJourneyRulesApi.dll --validate-only

FROM alpine AS final
WORKDIR /output
COPY --from=validated /app/configurations/output/ ./
ENTRYPOINT ["sh"]
