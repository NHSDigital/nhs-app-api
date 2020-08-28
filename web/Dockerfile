# syntax=docker/dockerfile:experimental

# run for hot reload of files while developing
FROM nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0 as dev-serve
RUN mkdir -p /data/web/node_modules
WORKDIR /data/web
CMD ["npm", "run", "dev"]

# Security Audit
FROM nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0 as security_audit

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
FROM nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0 AS production_dependencies

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

RUN if [ -n "$APP_VERSION_TAG" ] ; then \
        sed -i "s/dev_web_npm/$APP_VERSION_TAG/" package.json; \
    fi

RUN npm run build

# production stage
FROM nhsapp.azurecr.io/nhsonline-nginx:v1.18.0 as release
COPY --chown=www-data:www-data --from=build /app/dist /app
COPY --chown=www-data:www-data --from=build /app/build/env.sh /app/build/env.sh
COPY --chown=www-data:www-data nginx/nginx.conf.template /etc/nginx/nginx.conf.template

USER root

RUN apt-get update && \
    apt-get install --no-install-recommends -y sudo ca-certificates && \
    rm -rf /var/lib/apt/lists/* && \
    apt-get clean

RUN chown www-data:www-data /etc/nginx/nginx.conf
RUN echo "www-data ALL=(ALL) NOPASSWD: /usr/sbin/update-ca-certificates" >> /etc/sudoers.d/www-data && \
    chmod 400 /etc/sudoers.d/www-data

USER www-data

EXPOSE $PORT
CMD sudo update-ca-certificates; \
    sh /app/build/env.sh ./app/config.json; \
    envsubst '${PORT}' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf; \
    nginx;

# 1. trust wildcard cert (mounted in helm)to allow communication between pods
# 2. run env.sh script to create env.json for config endpoint
# 3. create nginx.conf file - replace ${PORT} variable with value defined in helm
# 4. start nginx

