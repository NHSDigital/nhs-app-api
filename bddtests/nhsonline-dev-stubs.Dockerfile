ARG WIREMOCK_VERSION=2.22.0

FROM nhsapp.azurecr.io/nhsonline-int-tests-base:jdk8-node12-1.0 as mappings

# ARG values get reset by FROM
ARG WIREMOCK_VERSION=2.22.0

USER root

ENV wiremockUrl=http://localhost:8080/

RUN DEBIAN_FRONTEND=noninteractive apt-get install -y tzdata curl && \
    ln -fs /usr/share/zoneinfo/Europe/London /etc/localtime && \
    dpkg-reconfigure --frontend noninteractive tzdata

RUN mkdir -p /var/wiremock/lib/ && \
    wget https://repo1.maven.org/maven2/com/github/tomakehurst/wiremock-jre8-standalone/$WIREMOCK_VERSION/wiremock-jre8-standalone-$WIREMOCK_VERSION.jar \
    -O /var/wiremock/lib/wiremock-jre8-standalone.jar

COPY . .

RUN java -cp /var/wiremock/lib/* com.github.tomakehurst.wiremock.standalone.WireMockServerRunner & \
    ./gradlew --no-daemon mock && \
    curl -X POST ${wiremockUrl}__admin/mappings/save

FROM nhsapp.azurecr.io/wiremock:${WIREMOCK_VERSION}-alpine

COPY --from=mappings /data/mappings mappings
