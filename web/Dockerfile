FROM nhsapp.azurecr.io/nhsonline-web-base AS base

COPY --chown=node:node package.json .
COPY --chown=node:node package-lock.json .
COPY --chown=node:node .npmrc .
COPY --chown=node:node lint/package.json lint/package.json

# Production Dependencies
FROM base AS production_dependencies
RUN npm set progress=false && npm config set depth 0
RUN npm install --only=production

# Build Dependencies
FROM production_dependencies as build_dependencies
RUN npm install

# Build
FROM build_dependencies as build
COPY --chown=node:node . .

ARG COMMIT_ID
RUN sed -i "s/.*COMMIT_ID:.*/    COMMIT_ID: '$COMMIT_ID',/" nuxt.config.js

ARG APP_VERSION_TAG
RUN if [ -n "$APP_VERSION_TAG" ] ; \
 then \
 sed -i "s/.*const APP_VERSION_TAG.*/const APP_VERSION_TAG = '$APP_VERSION_TAG';/" nuxt.config.js; \
 fi

RUN npm run build

# Tests
FROM build_dependencies AS test
COPY --chown=node:node . .

# Final image
FROM production_dependencies AS release
COPY --chown=node:node --from=build /opt/app/.nuxt ./.nuxt
COPY --chown=node:node --from=build /opt/app/src ./src
COPY --chown=node:node --from=build /opt/app/nuxt.config.js ./nuxt.config.js
RUN rm -r lint

EXPOSE $PORT
CMD sudo update-ca-certificates && exec node ./node_modules/nuxt/bin/nuxt start
