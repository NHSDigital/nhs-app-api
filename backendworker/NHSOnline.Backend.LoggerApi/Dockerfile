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
ARG PROJECT_NAME=NHSOnline.Backend.LoggerApi
ARG PROJECT_PLATFORM
ARG COMMIT_ID
WORKDIR /src
WORKDIR /src/${PROJECT_NAME}
RUN sed -i "s/.*\"CommitId\":.*/    \"CommitId\": \"$COMMIT_ID\"/" appsettings.json
RUN dotnet build --no-restore -c Release -o /app  -r ${PROJECT_PLATFORM}

FROM built AS publish
ARG PROJECT_PLATFORM
RUN dotnet publish --no-restore -c Release -o /app -r ${PROJECT_PLATFORM}

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-runtime AS final
ARG PROJECT_NAME=NHSOnline.Backend.LoggerApi
ARG PORT=8090

ENV PROJECT_NAME=$PROJECT_NAME
ENV PORT=$PORT
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=
ENV CORS_AUTHORITY https://www-preview.devuks1.bitraft.io
ENV TIMEZONE Europe/London
ENV CLIENT_LOGGER_LEVEL error

WORKDIR /app
RUN chown aspnetcore:aspnetcore /app
COPY --chown=aspnetcore:aspnetcore --from=publish /app .

USER aspnetcore
EXPOSE $PORT
CMD exec dotnet ${PROJECT_NAME}.dll