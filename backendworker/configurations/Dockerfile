FROM nhsonline.backend.servicejourneyrulesapi AS sjr
WORKDIR /app
COPY --chown=aspnetcore:aspnetcore configurations/gpinfo.csv ./configurations/gpinfo.csv
COPY --chown=aspnetcore:aspnetcore configurations/Journeys/ ./configurations/Journeys/
COPY --chown=aspnetcore:aspnetcore configurations/Rules/ ./configurations/Rules/

ENV GP_INFO_FILE_PATH ./configurations/gpinfo.csv
ENV CONFIGURATION_FOLDER_PATH ./configurations
ENV RULES_FOLDER_NAME Rules
ENV JOURNEYS_FOLDER_NAME Journeys
ENV OUTPUT_FOLDER_PATH ./configurations/output
RUN dotnet NHSOnline.Backend.ServiceJourneyRulesApi.dll --validate-only

FROM alpine AS final
WORKDIR /output
COPY --from=sjr /app/configurations/output/ ./
ENTRYPOINT ["sh"]
