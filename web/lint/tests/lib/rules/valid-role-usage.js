// ------------------------------------------------------------------------------
// Requirements
// ------------------------------------------------------------------------------

const { RuleTester } = require('eslint');
const rule = require('../../../lib/rules/valid-role-usage');

// ------------------------------------------------------------------------------
// Tests
// ------------------------------------------------------------------------------

const tester = new RuleTester({
  parser: 'vue-eslint-parser',
  parserOptions: { ecmaVersion: 2015 },
});

tester.run('html-end-tags', rule, {
  invalid: [
    {
      filename: 'test.vue',
      code: '<span role="text"/>',
      errors: ["'Should not use role=text, as flagged in accessibility report'"],
    },
  ],
});
