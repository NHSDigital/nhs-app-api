# syntax=docker/dockerfile:experimental

ARG DOCKER_IMAGE_WEB_BUILD=nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0
ARG DOCKER_IMAGE_WEB_RUNTIME=nhsapp.azurecr.io/nhsonline-nginx:v1.18.0

# run for hot reload of files while developing
FROM ${DOCKER_IMAGE_WEB_BUILD} as dev-serve
RUN mkdir -p /data/web/node_modules
WORKDIR /data/web
CMD ["npm", "run", "dev"]

# Security Audit
FROM ${DOCKER_IMAGE_WEB_BUILD} as security_audit

COPY .npmrc .
COPY package.json .
COPY package-lock.json .

COPY lint/.npmrc lint/.npmrc
COPY lint/package.json lint/package.json
COPY lint/package-lock.json lint/package-lock.json

RUN --mount=type=secret,id=npmrc,dst=/root/.npmrc \
    --mount=type=cache,id=npm,dst=/root/.npm \
    npm install -g npm-audit-ci-wrapper \
      && npm run audit \
      && cd lint \
      && npm run audit

# Production Dependencies
FROM ${DOCKER_IMAGE_WEB_BUILD} AS production_dependencies

WORKDIR /app

COPY .npmrc .
COPY package.json .
COPY package-lock.json .

RUN npm set progress=false \
  && npm config set depth 0

RUN --mount=type=secret,id=npmrc,dst=/root/.npmrc \
    --mount=type=cache,id=npm,dst=/root/.npm \
    npm install --only=production

# Build Dependencies
FROM production_dependencies as build_dependencies

COPY lint/.npmrc lint/.npmrc
COPY lint/package.json lint/package.json
COPY lint/package-lock.json lint/package-lock.json

RUN --mount=type=secret,id=npmrc,dst=/root/.npmrc \
    --mount=type=cache,id=npm,dst=/root/.npm \
    npm install \
      && cd lint \
      && npm install

RUN --mount=type=secret,id=npmrc,dst=/root/.npmrc \
    --mount=type=cache,id=npm,dst=/root/.npm \
    npm run outdated \
      && cd lint \
      && npm run outdated

# Lint
FROM build_dependencies as lint

COPY . .

RUN npm run lint

# Build
FROM lint as build

ARG COMMIT_ID
ARG APP_VERSION_TAG

RUN echo 'VUE_APP_COMMIT_ID='$COMMIT_ID >> /app/.env
RUN echo $APP_VERSION_TAG | sed 's/\./\-/g' > /app/dashed_version

# define the version number for the build - the static assets folder name, and the path to call the config.json
RUN DASHED_VERSION=$(cat /app/dashed_version); \
    sed -i "s/dev_web_npm/${DASHED_VERSION}/" package.json; \
    if [ -n "$DASHED_VERSION" ] ; then \
        DASHED_VERSION="\/$DASHED_VERSION"; \
    fi; \
    sed -i "s/CONFIG_PATH/${DASHED_VERSION}/" src/main.js;

RUN npm run build

# production stage
FROM ${DOCKER_IMAGE_WEB_RUNTIME} as release

# any scripts needing run at runtime must be saved with executable permissions
# - chmod +x <script name>
COPY --chown=www-data:www-data --from=build /app/build/docker-runtime/. /app/build/
COPY --chown=www-data:www-data --from=build /app/dist /app
COPY --chown=www-data:www-data --from=build /app/dashed_version /app/build/dashed_version
COPY --chown=www-data:www-data nginx/nginx.conf.template /etc/nginx/nginx.conf.template

USER root

RUN apt-get update && \
    apt-get install --no-install-recommends -y sudo ca-certificates jq && \
    rm -rf /var/lib/apt/lists/* && \
    apt-get clean

RUN chown www-data:www-data /etc/nginx/nginx.conf
RUN echo "www-data ALL=(ALL) NOPASSWD: /usr/sbin/update-ca-certificates" >> /etc/sudoers.d/www-data && \
    chmod 400 /etc/sudoers.d/www-data;

USER www-data

ARG COMMIT_ID
ENV COMMIT_ID=${COMMIT_ID}

EXPOSE $PORT
CMD sudo update-ca-certificates; \
    /app/build/env-wrapper.sh; \
    /app/build/configureDeepLinking.sh; \
    envsubst '${PORT}' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf; \
    nginx;

# 1. trust wildcard cert (mounted in helm)to allow communication between pods
# 2. run env-wrapper.sh script to create env.json for config endpoint and place in versioned assets folder
# 3. run configureDeepLinking.sh to configure the well-known files
# 4. create nginx.conf file - replace ${PORT} variable with value defined in helm
# 5. start nginx

