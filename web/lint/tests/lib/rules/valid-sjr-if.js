// ------------------------------------------------------------------------------
// Requirements
// ------------------------------------------------------------------------------

const { RuleTester } = require('eslint');
const rule = require('../../../lib/rules/valid-sjr-if');

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
      code: '',
    },
    {
      filename: 'test.vue',
      code: '<template><sjr-if journey="online-consultations"></sjr-if></template>',
    },
    {
      filename: 'test.vue',
      code: '<template><sjr-if journey="online-consultations" tag="span"></sjr-if></template>',
    },
  ],
  invalid: [
    {
      filename: 'test.vue',
      code: '<template><sjr-if></sjr-if></template>',
      errors: ["'<sjr-if>' should have a 'journey' attribute."],
    },
    {
      filename: 'test.vue',
      code: '<template><sjr-if tag="span"></sjr-if></template>',
      errors: ["'<sjr-if>' should have a 'journey' attribute."],
    },
    {
      filename: 'test.vue',
      code: '<template><sjr-if journey tag="span"></sjr-if></template>',
      errors: ["'<sjr-if>' missing 'journey' value."],
    },
    {
      filename: 'test.vue',
      code: '<template><sjr-if journey="on1ine-consultations" tag="span"></sjr-if></template>',
      errors: ["'on1ine-consultations' journey does not have a corresponding getter."],
    },
  ],
});
