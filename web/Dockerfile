FROM nhsapp.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 3000
ENV API_HOST https://api-preview.nonliveeuw.bitraft.io
ENV ORGAN_DONATION_URL https://www.organdonation.nhs.uk/
ENV DATA_SHARING_URL https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/
ENV SYMPTOM_CHECKER_URL https://111.nhs.uk

RUN chown nodejs:nodejs -R /opt/app
USER nodejs

COPY --chown=nodejs:nodejs package.json .
COPY --chown=nodejs:nodejs package-lock.json .
COPY --chown=nodejs:nodejs nuxt.config.js .
COPY --chown=nodejs:nodejs .babelrc .

# Deps
FROM base AS dependencies
RUN npm set progress=false && npm config set depth 0
RUN npm install --only=production
RUN cp -R node_modules prod_node_modules
RUN npm install

# Tests
FROM dependencies AS test
COPY . .
# RUN npm test

# Build
FROM dependencies as build
COPY . .

# Final image
FROM base AS release
COPY --chown=nodejs:nodejs --from=build /opt/app/prod_node_modules ./node_modules
COPY --chown=nodejs:nodejs --from=build /opt/app/build ./build
COPY --chown=nodejs:nodejs --from=build /opt/app/contracts ./contracts
COPY --chown=nodejs:nodejs --from=build /opt/app/src ./src
COPY --chown=nodejs:nodejs --from=build /opt/app/src/pages ./pages
COPY --chown=nodejs:nodejs --from=build /opt/app/app_links ./app_links
COPY --chown=nodejs:nodejs --from=build /opt/app/test ./test

EXPOSE 3000
CMD npm run dev
