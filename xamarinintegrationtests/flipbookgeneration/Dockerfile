FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive
ENV WKHTMLTOPDF_VERSION=0.12.6.1-2
ARG WKHTMLTOPDF_BUILD='jammy_amd64'

RUN if [ $(uname -m) = 'aarch64' ] ; then WKHTMLTOPDF_BUILD='jammy_arm64' ;echo ${WKHTMLTOPDF_BUILD} ; fi && \
    apt-get update && \
    apt-get install -y nodejs && \
    apt-get install -y wget && \
    wget https://github.com/wkhtmltopdf/packaging/releases/download/${WKHTMLTOPDF_VERSION}/wkhtmltox_${WKHTMLTOPDF_VERSION}.${WKHTMLTOPDF_BUILD}.deb  && \
    apt-get install -fy ./wkhtmltox_${WKHTMLTOPDF_VERSION}.${WKHTMLTOPDF_BUILD}.deb && \
    apt-get install -y xvfb 

WORKDIR /src

COPY generator.js ./
COPY generate.sh ./

COPY flipbook/ ./flipbook/

