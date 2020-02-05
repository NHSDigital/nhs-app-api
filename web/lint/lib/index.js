/* eslint-disable global-require */

module.exports = {
  rules: {
    'valid-sjr-if': require('./rules/valid-sjr-if'),
    'valid-lodash-import': require('./rules/valid-lodash-import'),
  },
  configs: {
    default: {
      plugins: [
        'custom',
      ],
      rules: {
        'custom/valid-sjr-if': 'error',
        'custom/valid-lodash-import': 'error',
      },
    },
  },
};
