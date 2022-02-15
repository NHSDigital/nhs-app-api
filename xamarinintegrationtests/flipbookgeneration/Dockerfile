FROM nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && \ 
    apt-get install -y wkhtmltopdf && \
    apt-get install -y xvfb

WORKDIR /src

COPY generator.js ./
COPY generate.sh ./

COPY flipbook/ ./flipbook/