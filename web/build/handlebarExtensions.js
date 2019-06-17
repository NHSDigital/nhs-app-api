const _ = require('lodash');

function handlebarsExt(Handlebars) {
  Handlebars.registerHelper('toUpperCase', str => str.toUpperCase());

  Handlebars.registerHelper('ifEquals', function (arg1, arg2, options) {
    return (arg1 == arg2) ? options.fn(this) : options.inverse(this);
  });

  Handlebars.registerHelper('ifTrue', function (arg1, options) {
    return (arg1 == true) ? options.fn(this) : options.inverse(this);
  });

  Handlebars.registerHelper('className', (str) => {
    const camelCaseString = _.camelCase(str.toString().replace(/\s/g, ''));
    return camelCaseString.charAt(0).toUpperCase() + camelCaseString.slice(1);
  });
}

module.exports = handlebarsExt;
