/* eslint-disable no-underscore-dangle */
/* eslint-disable import/no-extraneous-dependencies */
/* eslint-disable func-names */
/* eslint-disable global-require */

import Vue from 'vue';

export default ({ store }) => {
  const vueErrorLogHandler = function (err) {
    let errorMessage = err;
    if (err && err.stack) {
      errorMessage = err.stack;
    }
    store.dispatch('modules/log/onError', errorMessage);

    throw err;
  };

  const globalErrorLogHandler = function (message) {
    store.dispatch('modules/log/onError', message);
  };

  if (process.server) {
    const consola = require('consola');
    // Consola doesn't expose enough to be able to configure a custom reporter and so
    // we're altering its internals.
    // There is a further ticket (NHSO-3830) that will look to replace Consola
    consola._reporters[0].options.dateFormat = 'YYYY-MM-DD HH:mm:ss.SSSZ';
  } else {
    window.onerror = globalErrorLogHandler;
  }
  Vue.config.errorHandler = vueErrorLogHandler;
};
