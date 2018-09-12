FROM nhsapp.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 3000
ENV API_HOST https://api-preview.devuks1.bitraft.io
ENV ORGAN_DONATION_URL https://www.organdonation.nhs.uk/
ENV DATA_SHARING_URL https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/
ENV SYMPTOM_CHECKER_URL https://111.nhs.uk
ENV SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS ?referrer=nhsapp&utm_source=%20&utm_medium=NHS%20App&utm_campaign=%20
ENV ANALYTICS_SCRIPT_URL //assets.adobedtm.com/launch-EN2bcb86c8edd64d5aa2abd8aabdcfb129-development.min.js
ENV ANALYTICS_ENVIRONMENT development
ENV CONDITIONS_CHECKER_URL https://www.nhs.uk/conditions/
ENV TEMPORARY_ACCOUNT_URL http://beta.nhs.uk/
ENV TERMS_AND_CONDITIONS_URL https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/
ENV PRIVACY_POLICY_URL https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/
ENV COOKIES_POLICY_URL https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/
ENV OPEN_SOURCE_LICENSES_URL https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/
ENV HELP_AND_SUPPORT_URL https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/
ENV CLINICAL_ABBREVIATIONS_URL https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/medical-abbreviations
ENV HOT_JAR_URL https://in.hotjar.com/s?siteId=859152&surveyId=95785
ENV DATA_PREFERENCES_URL https://ndopapp-int1.thunderbird.service.nhs.uk/createsession

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

ENV NODE_ENV=production

EXPOSE $PORT
CMD sudo update-ca-certificates && npm run build && npm run start
