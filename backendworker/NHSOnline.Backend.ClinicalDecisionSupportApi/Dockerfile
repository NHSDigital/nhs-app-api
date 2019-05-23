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
WORKDIR /src/NHSOnline.Backend.ClinicalDecisionSupportApi
COPY NHSOnline.Backend.ClinicalDecisionSupportApi/NHSOnline.Backend.ClinicalDecisionSupportApi.csproj ./
RUN dotnet restore  -r linux-x64

FROM dependencies as built
WORKDIR /src
COPY . .
WORKDIR /src/NHSOnline.Backend.ClinicalDecisionSupportApi

RUN dotnet build -c Release -o /app  -r linux-x64

FROM built AS publish
RUN dotnet publish -c Release -o /app -r linux-x64

FROM runtime AS final
ENV PORT=8087
ENV ASPNETCORE_URLS=
ENV AUDIT_SINK_TYPE FILE
ENV SESSION_MONGO_DATABASE_NAME development
ENV OnlineConsultationsProvidersSettings__Providers__0__Provider=eConsult
ENV OnlineConsultationsProvidersSettings__Providers__0__BaseAddress=http://ems.cdss.stubs.local.bitraft.io:8088/
ENV OnlineConsultationsProvidersSettings__Providers__0__BearerToken=shfjlkasdhflashfl

WORKDIR /app
EXPOSE $PORT
COPY --from=publish /app .

RUN chown aspnetcore:aspnetcore -R /app
USER aspnetcore
CMD exec dotnet NHSOnline.Backend.ClinicalDecisionSupportApi.dll
