# Dockerfile (tag: v3)
FROM node:8.9.4
RUN npm install webpack -g
RUN npm install webpack-dev-server -g

COPY package.json /tmp/

WORKDIR /tmp
RUN npm config set registry http://registry.npmjs.org/ && npm install

COPY . /app

WORKDIR /app
RUN cp -r /tmp/node_modules /app

EXPOSE 8080

CMD ["npm", "run", "dev"]
