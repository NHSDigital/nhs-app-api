# Production Dependencies
FROM nhsapp.azurecr.io/nhsonline-nodejs-build:node8-1.0 AS production_dependencies

COPY package.json .
COPY package-lock.json .

RUN npm set progress=false && npm config set depth 0
RUN npm install --only=production

# Build Dependencies
FROM production_dependencies as build_dependencies

COPY .npmrc .
COPY lint/package.json lint/package.json

RUN npm install

# Build
FROM build_dependencies as build

ARG COMMIT_ID
ARG APP_VERSION_TAG

COPY . .

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
FROM nhsapp.azurecr.io/nhsonline-nodejs-runtime:node8-1.0 AS release

COPY --chown=node:node --from=production_dependencies /data/package.json ./package.json
COPY --chown=node:node --from=production_dependencies /data/package-lock.json ./package-lock.json
COPY --chown=node:node --from=production_dependencies /data/node_modules ./node_modules
COPY --chown=node:node --from=build /data/.nuxt ./.nuxt
COPY --chown=node:node --from=build /data/src ./src
COPY --chown=node:node --from=build /data/nuxt.config.js ./nuxt.config.js

EXPOSE $PORT
CMD sudo update-ca-certificates && exec node ./node_modules/nuxt/bin/nuxt start
