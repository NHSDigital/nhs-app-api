FROM nhsonline.azurecr.io/nhsonline-web-base:latest AS base

ENV HOST 0.0.0.0
ENV PORT 4000
ENV API_HOST http://localhost:8800
ENV ORGAN_DONATION_URL https://www.organdonation.nhs.uk/
ENV SYMPTOM_CHECKER_URL https://111.nhs.uk

COPY package.json .
COPY package-lock.json .

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
RUN npm run build

# Final image
FROM base AS release
COPY --from=build /opt/app/prod_node_modules ./node_modules
COPY --from=build /opt/app/dist ./dist
COPY --from=build /opt/app/config ./config
COPY --from=build /opt/app/server.js .
COPY --from=build /opt/app/app_links ./app_links
RUN chown nodejs:nodejs -R /opt/app
USER nodejs
EXPOSE 4000
CMD npm run serve
