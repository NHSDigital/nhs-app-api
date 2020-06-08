/* eslint-disable global-require */

module.exports = {
  rules: {
    'valid-sjr-if': require('./rules/valid-sjr-if'),
    'valid-lodash-import': require('./rules/valid-lodash-import'),
    'valid-target-usage': require('./rules/valid-target-usage'),
    'invalid-tags': require('./rules/invalid-tags'),
  },
  configs: {
    default: {
      plugins: [
        'custom',
      ],
      rules: {
        'custom/valid-sjr-if': 'error',
        'custom/valid-lodash-import': 'error',
        'custom/valid-target-usage': 'error',
        'custom/invalid-tags': 'error',
      },
    },
  },
};
