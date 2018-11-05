FROM nhsapp.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 3000
ENV NODE_TLS_REJECT_UNAUTHORIZED 0

RUN chown nodejs:nodejs -R /opt/app
USER nodejs

COPY --chown=nodejs:nodejs package.json .
COPY --chown=nodejs:nodejs package-lock.json .
COPY --chown=nodejs:nodejs .npmrc .

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
RUN npm run build

# Tests
FROM build_dependencies AS test
COPY --chown=nodejs:nodejs . .


# Final image
FROM production_dependencies AS release
COPY --chown=nodejs:nodejs --from=build /opt/app/.nuxt ./.nuxt
COPY --chown=nodejs:nodejs --from=build /opt/app/src ./src
COPY --chown=nodejs:nodejs --from=build /opt/app/nuxt.config.js ./nuxt.config.js

EXPOSE $PORT
CMD sudo update-ca-certificates && ./node_modules/nuxt/bin/nuxt start
