const fs = require('fs');
const path = require('path');
const yaml = require('js-yaml').safeLoad;
const { CodeGen } = require('swagger-js-codegen');

module.exports = ({ contractPath, outputPath }) => {
  const swagger = yaml(fs.readFileSync(contractPath, 'UTF-8'));

  const generatedSourceCode = CodeGen.getCustomCode({
    swagger,
    className: 'NHSOnlineApi',
    template: {
      class: fs.readFileSync(path.join(__dirname, 'template/es6-class.mustache'), 'UTF-8'),
      method: fs.readFileSync(path.join(__dirname, 'template/es6-method.mustache'), 'UTF-8'),
    },
  });

  fs.writeFileSync(outputPath, generatedSourceCode);
};
