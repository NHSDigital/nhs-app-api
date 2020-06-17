const fs = require('fs');
const env = require('./env.js');

const data = JSON.stringify(env);
fs.writeFileSync('./app/config', data);
