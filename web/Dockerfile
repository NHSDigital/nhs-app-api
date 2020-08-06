# syntax=docker/dockerfile:experimental

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

## add version info to config
RUN sed -i "s/.*COMMIT_ID:.*/    COMMIT_ID: '$COMMIT_ID',/" nuxt.config.js && \
  if [ -n "$APP_VERSION_TAG" ] ; then \
    sed -i "s/.*const APP_VERSION_TAG.*/const APP_VERSION_TAG = '$APP_VERSION_TAG';/" nuxt.config.js; \
  fi

RUN npm run build

# Tests
FROM build_dependencies AS test
COPY . .

# Final image
FROM nhsapp.azurecr.io/nhsonline-nodejs-runtime:node14-1.0 AS release

COPY --chown=node:node --from=production_dependencies /data/package.json ./package.json
COPY --chown=node:node --from=production_dependencies /data/package-lock.json ./package-lock.json
COPY --chown=node:node --from=production_dependencies /data/node_modules ./node_modules
COPY --chown=node:node --from=build /data/.nuxt ./.nuxt
COPY --chown=node:node --from=build /data/src ./src
COPY --chown=node:node --from=build /data/nuxt.config.js ./nuxt.config.js

EXPOSE $PORT
CMD sudo update-ca-certificates && exec node ./node_modules/nuxt/bin/nuxt start
