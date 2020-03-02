// ------------------------------------------------------------------------------
// Requirements
// ------------------------------------------------------------------------------

const { RuleTester } = require('eslint');
const rule = require('../../../lib/rules/valid-target-usage');

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
      code: '<template><a target="_blank" rel="noopener noreferrer"></a></template>',
    },
    {
      filename: 'test.vue',
      code: '<template><a target="_self"></a></template>',
    },
  ],
  invalid: [
    {
      filename: 'test.vue',
      code: '<template><a target="_blank"></a></template>',
      errors: ['Should have a rel attribute with value of "noopener noreferrer"'],
    },
    {
      filename: 'test.vue',
      code: '<template><menu-item target="_blank" rel="value"></menu-item></template>',
      errors: ['rel attribute should have a value of "noopener noreferrer"'],
    },
  ],
});
