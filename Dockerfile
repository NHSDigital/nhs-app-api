FROM nhsonline.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 3000
ENV API_HOST https://develop.api.bitraft.io
ENV ORGAN_DONATION_URL https://www.organdonation.nhs.uk/
ENV SYMPTOM_CHECKER_URL https://111.nhs.uk

COPY package.json .
COPY package-lock.json .
COPY nuxt.config.js .

# Deps
FROM base AS dependencies
RUN npm set progress=false && npm config set depth 0
RUN npm install --only=production
RUN cp -R node_modules prod_node_modules
RUN npm install

# Tests
FROM dependencies AS test
COPY . .
#RUN npm test

# Build
FROM dependencies as build
COPY . .

# Final image
FROM base AS release
COPY --from=build /opt/app/prod_node_modules ./node_modules
COPY --from=build /opt/app/build ./build
COPY --from=build /opt/app/contracts ./contracts
COPY --from=build /opt/app/src ./src
COPY --from=build /opt/app/src/pages ./pages
COPY --from=build /opt/app/app_links ./app_links
RUN chown nodejs:nodejs -R /opt/app
USER nodejs
EXPOSE 3000
CMD npm run dev
