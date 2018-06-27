// The 'no-console' rule is disable as, for a console app, logging to the console is appropriate.
/* eslint-disable no-console */

const minimist = require('minimist');
const generate = require('./swagger-codegen/generate');

const options = minimist(process.argv.slice(2));
const contractPath = options.contract_path;
const outputPath = options.output_path;

console.log('Generating Swagger Code:');
console.log(`  contract path: ${contractPath}`);
console.log(`  output path:   ${outputPath}`);
generate({ contractPath, outputPath });
