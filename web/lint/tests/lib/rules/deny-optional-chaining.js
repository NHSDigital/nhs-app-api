// ------------------------------------------------------------------------------
// Requirements
// ------------------------------------------------------------------------------

const { RuleTester } = require('eslint');
const rule = require('../../../lib/rules/deny-optional-chaining');

// ------------------------------------------------------------------------------
// Tests
// ------------------------------------------------------------------------------

const tester = new RuleTester({
  parser: 'vue-eslint-parser',
  parserOptions: { ecmaVersion: 2015 },
});

tester.run('deny-optional-chaining', rule, {
  valid: [
    {
      filename: 'test.vue',
      code: 'foo.bar',
    },
  ],
  invalid: [
    {
      filename: 'test.vue',
      code: 'foo?.bar',
      errors: [{ message:"Pingdom currently doesn't support Optional chaining so lets not use it for now. If you are seeing this make sure and check if that's still the case."}],
    },
  ],
});
