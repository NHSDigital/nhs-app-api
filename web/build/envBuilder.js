const { execSync } = require('child_process');
const { readFileSync, writeFileSync } = require('fs');
const { exit } = require('process');

const prefix = '.';
const outputFilePath = '.env';

const errorAndExit = msg => {
  console.error(`${__filename} => ${msg}`);

  exit(1);
}

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

const writeEnvFileFromDockerAndExit = () => {
  const errorAndExitDocker = msg =>
    errorAndExit(`Error getting web env JSON from docker: ${msg}\nIs the local environment running?`);

    try {
      const webContainerId = 
          execSync(`docker ps | grep web.local.bitraft | cut -d' ' -f1`).toString().trim();

      if (webContainerId === '') {
        errorAndExitDocker('no web container detected');
      }

      const webEnvDocker = 
          execSync(`docker inspect --format='{{range .Config.Env}}{{println .}}{{end}}' ${webContainerId}`).toString().trim();

      writeFileSync(outputFilePath, `${webEnvDocker}`);

      console.log(`${__filename} => Wrote web env vars to ${outputFilePath}`);

      exit(0);

    } catch (ex) {
    errorAndExitDocker(`${ex}`);
    }
}

const writeEnvFileAndExit = webEnv => {
  const webEnvString = convertToEnvString(webEnv)

  writeFileSync(outputFilePath, `${webEnvString}`);

  console.log(`${__filename} => Wrote web env vars to ${outputFilePath}`);

  exit(0);
}

const main = () => {
  const args = process.argv.map(a => a.toLowerCase().trim());

  if (args.find(a => a === '--docker')) {
    writeEnvFileFromDockerAndExit();

    return;
  }

  var envFiles = [];
  findEnvFilePaths('./docker-compose.yml', envFiles);

  var envFile = {};
  populateEnvFile(envFiles, envFile);

  writeEnvFileAndExit(envFile);
}

function convertToEnvString (object) {
  let envFile = ''
  for (const key of Object.keys(object)) {
      envFile += `${key}=${object[key]}\n`
  }
  return envFile
}

main()