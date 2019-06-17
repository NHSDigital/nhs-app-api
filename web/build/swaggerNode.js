const minimist = require('minimist');
const path = require('path');
const codegen = require('swagger-node-codegen');

const options = minimist(process.argv.slice(2));
const contractPath = options.contract_path;
const outputPath = options.output_path;
const nhsOnlineTemplatesPath = options.nhsonline_templates_path;
const handlebarsExtPath = options.handlebars_path;

console.log('Generating api code -->');

codegen.generate({
  swagger: path.resolve(__dirname, contractPath),
  target_dir: path.resolve(__dirname, outputPath),
  templates: path.resolve(__dirname, nhsOnlineTemplatesPath),
  handlebars_helper: path.resolve(__dirname, handlebarsExtPath),
}).then(() => {
  console.log('Done!');
}).catch((err) => {
  console.error(`Something went wrong: ${err.message}`);
});



