const fs = require('fs');
const path = require('path');
const CodeGen = require('swagger-js-codegen').CodeGen;
const yaml = require('js-yaml').safeLoad;

class SwaggerCodegenPlugin {
  constructor(options) {
    this.contractPath = options.contractPath;
    this.outputPath = options.outputPath;
  }

  apply(compiler) {
    compiler.plugin('compile', () => {
      const swagger = yaml(fs.readFileSync(this.contractPath, 'UTF-8'));

      const generatedSourceCode = CodeGen.getCustomCode({
        className: 'NHSOnlineApi',
        swagger,
        template: {
          class: fs.readFileSync(path.join(__dirname, 'template/es6-class.mustache'), 'UTF-8'),
          method: fs.readFileSync(path.join(__dirname, 'template/es6-method.mustache'), 'UTF-8'),
        },
      });

      fs.writeFileSync(this.outputPath, generatedSourceCode);
    });
  }
}

module.exports = SwaggerCodegenPlugin;
