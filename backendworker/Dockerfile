ARG PROJECT_PLATFORM=linux-x64

FROM nhsapp.azurecr.io/nhsonline-dotnetcore-build as dependencies
ARG PROJECT_PLATFORM
WORKDIR /src
COPY *.sln ./
COPY */*.csproj ./
RUN for file in *.csproj; do mkdir "${file%.*}" && mv "$file" "${file%.*}"; done
RUN dotnet restore -r ${PROJECT_PLATFORM}


FROM dependencies as built
ARG PROJECT_PLATFORM
ARG COMMIT_ID
RUN : "${COMMIT_ID:?COMMIT_ID argument needs to be set and non-empty.}"
WORKDIR /src
COPY . .
RUN sed -i "s/.*\"CommitId\":.*/    \"CommitId\": \"$COMMIT_ID\"/" */appsettings.json
RUN dotnet build --no-restore -c Release -r ${PROJECT_PLATFORM}


FROM built AS published
ARG PROJECT_PLATFORM
RUN dotnet publish --no-build -c Release -o _published_artifacts -r ${PROJECT_PLATFORM}


FROM nhsapp.azurecr.io/nhsonline-dotnetcore-runtime AS final
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
