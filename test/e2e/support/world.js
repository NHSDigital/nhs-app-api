import minimist from 'minimist';
import { client } from 'nightwatch-cucumber';
import { setWorldConstructor } from 'cucumber';

import config from '../../../config';

const commandLineOptions = minimist(process.argv.slice(2));

class World {
  constructor() {
    this.browser = client;
    this.baseUrl = commandLineOptions.base_url || (`http://localhost:${process.env.PORT || config.dev.port}`);
  }
}

setWorldConstructor(World);
