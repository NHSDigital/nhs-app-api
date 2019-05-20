FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y netcat sudo && \
    rm -rf /var/lib/apt/lists/* && \
    useradd -m aspnetcore

FROM microsoft/dotnet:2.1-sdk AS build
RUN apt-get update && \
    apt-get upgrade -y && \
    rm -rf /var/lib/apt/lists/*

FROM build as dependencies
WORKDIR /src
COPY *.sln ./
WORKDIR /src/NHSOnline.Backend.ServiceJourneyRulesApi
COPY NHSOnline.Backend.ServiceJourneyRulesApi/NHSOnline.Backend.ServiceJourneyRulesApi.csproj ./
RUN dotnet restore  -r linux-x64

FROM dependencies as built
WORKDIR /src
COPY . .
WORKDIR /src/NHSOnline.Backend.ServiceJourneyRulesApi

RUN dotnet build -c Release -o /app  -r linux-x64
WORKDIR /src
COPY configurations/ /app/configurations/
WORKDIR /src/NHSOnline.Backend.ServiceJourneyRulesApi

FROM built AS publish
RUN dotnet publish -c Release -o /app -r linux-x64

FROM runtime AS final
ENV PORT=8086
ENV ASPNETCORE_URLS=
ENV AUDIT_SINK_TYPE FILE
ENV SESSION_MONGO_DATABASE_NAME development
ENV GP_INFO_FILE_PATH ./configurations/gpinfo.csv
ENV CONFIGURATION_FOLDER_PATH ./configurations
ENV RULES_FOLDER_NAME Rules
ENV JOURNEYS_FOLDER_NAME Journeys
ENV OUTPUT_FOLDER_PATH ./configurations/output

WORKDIR /app
EXPOSE $PORT
COPY --from=publish /app .

ARG SJR_ARGS
ENV ENV_SJR_ARGS=$SJR_ARGS

RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD exec dotnet NHSOnline.Backend.ServiceJourneyRulesApi.dll $ENV_SJR_ARGS
