FROM nhsapp.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 3000
ENV API_HOST https://api-preview.devuks1.bitraft.io
ENV ORGAN_DONATION_URL https://www.organdonation.nhs.uk/
ENV DATA_SHARING_URL https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/
ENV SYMPTOM_CHECKER_URL https://111.nhs.uk
ENV ANALYTICS_SCRIPT_URL //assets.adobedtm.com/launch-EN2bcb86c8edd64d5aa2abd8aabdcfb129-development.min.js
ENV ANALYTICS_ENVIRONMENT development
ENV CONDITIONS_CHECKER_URL https://www.nhs.uk/conditions/
ENV TEMPORARY_ACCOUNT_URL http://beta.nhs.uk/
ENV TERMS_AND_CONDITIONS_URL http://beta.nhs.uk/
ENV PRIVACY_POLICY_URL http://beta.nhs.uk/
ENV COOKIES_POLICY_URL http://beta.nhs.uk/
ENV OPEN_SOURCE_LICENSES_URL http://beta.nhs.uk/
ENV HELP_AND_SUPPORT_URL http://beta.nhs.uk/

RUN chown nodejs:nodejs -R /opt/app
USER nodejs

COPY --chown=nodejs:nodejs package.json .
COPY --chown=nodejs:nodejs package-lock.json .
COPY --chown=nodejs:nodejs nuxt.config.js .
COPY --chown=nodejs:nodejs .babelrc .
COPY --chown=nodejs:nodejs .npmrc .

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

EXPOSE $PORT
CMD npm run dev
