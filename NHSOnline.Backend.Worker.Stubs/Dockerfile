FROM microsoft/dotnet:2.0-runtime AS runtime
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y netcat && \
    rm -rf /var/lib/apt/lists/* && \
    curl -L -o /usr/local/bin/wait-for https://raw.githubusercontent.com/eficode/wait-for/master/wait-for && \
    chmod +rx /usr/local/bin/wait-for

FROM microsoft/dotnet:2.0-sdk AS build
RUN apt-get update && \
    apt-get upgrade -y && \
    rm -rf /var/lib/apt/lists/*

FROM build as built
WORKDIR /src
COPY . .
WORKDIR /src/NHSOnline.Backend.Worker.Mocking
RUN dotnet build
WORKDIR /src/NHSOnline.Backend.Worker.Stubs
RUN dotnet restore
RUN dotnet build
RUN dotnet publish -c Release -o /app


FROM runtime as final
ENV WIREMOCK_AUTHORITY=nhsonline.stubs:8080
ENV WAIT_FOR_TIMEOUT=60
WORKDIR /app
COPY --from=built /app .
CMD wait-for -t ${WAIT_FOR_TIMEOUT} ${WIREMOCK_AUTHORITY} -- dotnet NHSOnline.Backend.Worker.Stubs.dll