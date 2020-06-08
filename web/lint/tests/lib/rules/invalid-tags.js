// ------------------------------------------------------------------------------
// Requirements
// ------------------------------------------------------------------------------

const { RuleTester } = require('eslint');
const rule = require('../../../lib/rules/invalid-tags');

// ------------------------------------------------------------------------------
// Tests
// ------------------------------------------------------------------------------

const tester = new RuleTester({
  parser: 'vue-eslint-parser',
  parserOptions: { ecmaVersion: 2015 },
});

tester.run('html-end-tags', rule, {
  valid: [
    {
      filename: 'test.vue',
      code: '<br>',
    },
  ],
  invalid: [
    {
      filename: 'test.vue',
      code: '<b>',
      errors: ["'<b>' tags do not have a sematic meaning. Please use a header tag, or a strong tag."],
    },
    {
      filename: 'test.vue',
      code: '<b v-if="index<anchorLinks.length-1" aria-hidden="true">',
      errors: ["'<b>' tags do not have a sematic meaning. Please use a header tag, or a strong tag."],
    },
  ],
});
