/* eslint-disable */
export default () => {
  if (process.server) {
    const consola = require('consola');
    // Consola doesn't expose enough to be able to configure a custom reporter and so we're altering its internals.
    // There is a further ticket (NHSO-3830) that will look to replace Consola
    consola._reporters[0].options.dateFormat = 'YYYY-MM-DD HH:mm:ss.SSSZ';
  }
};
