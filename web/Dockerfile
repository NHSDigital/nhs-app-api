FROM nhsapp.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 3000
ENV NODE_TLS_REJECT_UNAUTHORIZED 0

RUN chown nodejs:nodejs -R /opt/app
USER nodejs

COPY --chown=nodejs:nodejs package.json .
COPY --chown=nodejs:nodejs package-lock.json .
COPY --chown=nodejs:nodejs .npmrc .
COPY --chown=nodejs:nodejs lint/package.json lint/package.json

# Production Dependencies
FROM base AS production_dependencies
RUN npm set progress=false && npm config set depth 0
RUN npm install --only=production

# Build Dependencies
FROM production_dependencies as build_dependencies
RUN npm install


# Build
FROM build_dependencies as build
COPY --chown=nodejs:nodejs . .

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
COPY --chown=nodejs:nodejs . .


# Final image
FROM production_dependencies AS release
COPY --chown=nodejs:nodejs --from=build /opt/app/.nuxt ./.nuxt
COPY --chown=nodejs:nodejs --from=build /opt/app/src ./src
COPY --chown=nodejs:nodejs --from=build /opt/app/nuxt.config.js ./nuxt.config.js
RUN rm -r lint

EXPOSE $PORT
CMD sudo update-ca-certificates && exec node ./node_modules/nuxt/bin/nuxt start
