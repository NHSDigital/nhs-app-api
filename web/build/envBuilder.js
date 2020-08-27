var fs = require('fs');

const prefix = '.';

const findEnvFilePaths = (composeFilePath, outArray) => {

  var lines = fs.readFileSync(prefix + composeFilePath).toString().split('\n');
  var foundWebSection = false;
  var foundWebEnvSection = false;

  for (var i=0, max=lines.length; i<max; i++) {
    var line = lines[i].trim();
    if (line === 'web.local.bitraft.io:') {
      foundWebSection = true;
    }
    else if (foundWebSection && !foundWebEnvSection) {
      if (line === 'env_file:') {
        foundWebEnvSection = true;
      }
    }
    else if (foundWebSection && foundWebEnvSection) {
      if (line[0] == '-' ) {
        var boom = line.substr(2);
        outArray.push(boom);
      }
      else {
        break;
      }
    }
  }
};

const populateEnvFile = (filePaths, total) => {
  filePaths.forEach(r => {
    var envs = fs.readFileSync(prefix + r).toString().split('\n');
    envs.forEach(e => {
      var kv = e.split('=');
      total[kv[0]] = kv[1];
    })
  })
};

var envFiles = [];
findEnvFilePaths('./docker-compose.yml', envFiles);

var envFile = {};
populateEnvFile(envFiles, envFile);

const data = JSON.stringify(envFile);
fs.writeFileSync('src/config/env.json', data);
