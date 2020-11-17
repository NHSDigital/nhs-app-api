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

const AddNumericConfig = (variable, value) => {
  const numericValue = parseInt( value, 10 );
  if (isNaN(numericValue)) {
    console.log(`Failed AddNumericConfig for ${variable}=${value}`);
    process.exit(1);
  }
  return numericValue;
};

const AddBoolConfig = (variable, value) => {
  const lowerCase = value.toLowerCase();
  if (lowerCase !== 'true' && lowerCase !== 'false') {
    console.log(`Failed AddBoolConfig for ${variable}=${value}`);
    process.exit(1);
  }
  return (lowerCase === 'true');
};

const AddStringConfig = (variable, value) => {
  if (value === undefined || value === '') {
    console.log(`Failed AddStringConfig for ${variable}=${value}`);
    process.exit(1);
  }
  return value;
};

const determineEnvType = (envSh, config, formattedConfig) => {
  var lines = fs.readFileSync(envSh).toString().split('\n');
  var foundStart = false;

  for (var i=0, max=lines.length; i<max; i++) {
    var line = lines[i].trim();

    if (line[0] === '#') {
      continue;
    }
    else if (line === 'echo "Begin Generating web config json"') {
      foundStart = true;
    }
    else if (line === 'echo "Completed Generating web config json"') {
      break;
    }
    else if (foundStart) {
      var command = (line.split(';')[0]).split(' ');
      var envVar = command[1];
      var rawValue = config[envVar];

      if (command[0] === 'AddNumericConfig') {
        formattedConfig[envVar] = AddNumericConfig(envVar, rawValue);
      }
      else if (command[0] === 'AddBoolConfig') {
        formattedConfig[envVar] = AddBoolConfig(envVar, rawValue);
      }
      else {
        formattedConfig[envVar] = AddStringConfig(envVar, rawValue);
      }
    }
  }
}

var envFiles = [];
findEnvFilePaths('./docker-compose.yml', envFiles);

var envFile = {};
populateEnvFile(envFiles, envFile);

var typedEnvFile = {};
determineEnvType('build/docker-runtime/env.sh', envFile, typedEnvFile);

const data = JSON.stringify(typedEnvFile);
fs.writeFileSync('src/config/env.json', data);
