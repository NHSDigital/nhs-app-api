ARG PROJECT_PLATFORM=linux-x64

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-build as dependencies
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

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-runtime AS final
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
