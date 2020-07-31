/* eslint-disable global-require */

module.exports = {
  rules: {
    'valid-sjr-if': require('./rules/valid-sjr-if'),
    'valid-lodash-import': require('./rules/valid-lodash-import'),
    'valid-message-dialog': require('./rules/valid-message-dialog'),
    'valid-target-usage': require('./rules/valid-target-usage'),
    'valid-role-usage': require('./rules/valid-role-usage'),
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
        'custom/valid-message-dialog': 'error',
        'custom/valid-target-usage': 'error',
        'custom/valid-role-usage': 'error',
        'custom/invalid-tags': 'error',
      },
    },
  },
};
