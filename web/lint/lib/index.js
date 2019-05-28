/* eslint-disable global-require */

module.exports = {
  rules: {
    'valid-sjr-if': require('./rules/valid-sjr-if'),
  },
  configs: {
    default: {
      plugins: [
        'custom',
      ],
      rules: {
        'custom/valid-sjr-if': 'error',
      },
    },
  },
};
