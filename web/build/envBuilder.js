const { execSync } = require('child_process');
const { readFileSync, writeFileSync } = require('fs');
const { exit } = require('process');

const prefix = '.';
const outputFilePath = 'src/config/env.json';
const numberRegex = RegExp(/^\d+$/);
const booleanRegex = RegExp(/^(true|false)$/);
const dockerEnvRegex = RegExp(/^([^=]+)=(.*)$/);

const errorAndExit = msg => {
  console.error(`${__filename} => ${msg}`);

  exit(1);
}

const isNonBlankString = val => typeof val === 'string' && val.trim() !== '';

const isBooleanString = val => isNonBlankString(val) && booleanRegex.test(val.toLowerCase().trim());

const isNumberString = val => isNonBlankString(val) && numberRegex.test(val.toLowerCase().trim());

const findEnvFilePaths = (composeFilePath, outArray) => {
  const lines = readFileSync(prefix + composeFilePath).toString().split('\n');
  var foundWebSection = false;
  var foundWebEnvSection = false;
  var finished = false;

  lines.forEach(rawLine => {
    if (finished) {
      return;
    }

    var line = rawLine.trim();

    if (line === 'web.local.bitraft.io:') {
      foundWebSection = true;
    } else if (foundWebSection && !foundWebEnvSection && line === 'env_file:') {
      foundWebEnvSection = true;
    } else if (foundWebSection && foundWebEnvSection) {
      if (line[0] == '-' ) {
        var boom = line.substr(2);
        outArray.push(boom);
      } else {
        finished = true;
      }
    }
  });
};

const populateEnvFile = (filePaths, total) => {
  filePaths.forEach(r => {
    const envs = readFileSync(prefix + r).toString().split('\n');

    envs.forEach(e => {
      const [ k, v ] = e.split('=');

      total[k] = v;
    });
  })
};

const AddNumericConfig = (variable, value) => {
  if (!isNumberString(value)) {
    errorAndExit(`Failed AddNumericConfig for ${variable}=${value}`);
  }

  return parseInt(value, 10);
};

const AddBoolConfig = (variable, value) => {
  if (!isBooleanString(value)) {
    errorAndExit(`Failed AddBoolConfig for ${variable}=${value}`);
  }

  return (value.toLowerCase().trim() === 'true');
};

const AddStringConfig = (variable, value) => {
  if (!isNonBlankString(value)) {
    errorAndExit(`Failed AddStringConfig for ${variable}=${value}`);
  }

  return value;
};

const determineEnvType = (envSh, config, formattedConfig) => {
  const lines = readFileSync(envSh).toString().split('\n');
  let foundStart = false;
  let finished = false;

  lines.forEach(rawLine => {
    if (finished) {
      return;
    }

    const line = rawLine.trim();

    if (line[0] === '#') {
      return;
    } else if (line === 'echo "Begin Generating web config json"') {
      foundStart = true;
    } else if (line === 'echo "Completed Generating web config json"') {
      finished = true;

      return;
    } else if (foundStart) {
      const [ command, envVar ] = (line.split(';')[0]).split(' ');
      const rawValue = config[envVar];

      if (command === 'AddNumericConfig') {
        formattedConfig[envVar] = AddNumericConfig(envVar, rawValue);
      } else if (command === 'AddBoolConfig') {
        formattedConfig[envVar] = AddBoolConfig(envVar, rawValue);
      } else {
        formattedConfig[envVar] = AddStringConfig(envVar, rawValue);
      }
    }
  });
};

const writeEnvFileAndExit = webEnv => {
  const webEnvJson = JSON.stringify(webEnv, null, 2);

  writeFileSync(outputFilePath, `${webEnvJson}\n`);

  console.log(`${__filename} => Wrote web env JSON to ${outputFilePath}`);

  exit(0);
}

const parseDockerEnvVar = (webEnv, [ _, key, val ]) => {
  let parsedVal;

  if (isNumberString(val)) {
    parsedVal = parseInt(val, 10);
  } else if (isBooleanString(val)) {
    parsedVal = val.toLowerCase().trim() === 'true';
  } else if (isNonBlankString(val)) {
    parsedVal = val;
  } else {
    console.warn(`Empty value detected for env var: ${key}`);

    return;
  }

  webEnv[key] = parsedVal;
}

const createEnvJsonFromDockerContainer = () => {
  const errorAndExitDocker = msg =>
    errorAndExit(`Error getting web env JSON from docker: ${msg}\nIs the local environment running?`);

  try {
    const webContainerId = 
      execSync(`docker ps | grep web.local.bitraft | cut -d' ' -f1`).toString().trim();

    if (webContainerId === '') {
      errorAndExitDocker('no web container detected');
    }

    const webEnvDockerJson = 
      execSync(`docker inspect --format='{{json .Config.Env}}' ${webContainerId}`).toString().trim();

    if (webEnvDockerJson === '') {
      errorAndExitDocker('web container inspect returned no data');
    }

    const webEnv = {};

    JSON.parse(webEnvDockerJson)
      .filter(e => dockerEnvRegex.test(e))
      .map(e => dockerEnvRegex.exec(e))
      .forEach(m => parseDockerEnvVar(webEnv, m));

    writeEnvFileAndExit(webEnv);
   } catch (ex) {
    errorAndExitDocker(`${ex}`);
   }
};

const main = () => {
  const args = process.argv.map(a => a.toLowerCase().trim());

  if (args.find(a => a === '--docker')) {
    createEnvJsonFromDockerContainer();

    return;
  }

  var envFiles = [];
  findEnvFilePaths('./docker-compose.yml', envFiles);

  var envFile = {};
  populateEnvFile(envFiles, envFile);

  var typedEnvFile = {};
  determineEnvType('build/docker-runtime/env.sh', envFile, typedEnvFile);

  writeEnvFileAndExit(typedEnvFile);
}

main()
