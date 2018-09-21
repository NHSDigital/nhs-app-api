/* eslint-disable */
import NhsWebReporter from '@/logging/nhsWebReporter'

export default () => {
  if (process.server) {
    const consola = require('consola');
    consola.clear().add(new NhsWebReporter());
  }
};
