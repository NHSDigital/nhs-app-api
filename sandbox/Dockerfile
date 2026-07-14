FROM node:24

COPY . /sandbox

WORKDIR /sandbox

RUN npm install --ignore-scripts

EXPOSE 9000

CMD ["npm", "start"]
