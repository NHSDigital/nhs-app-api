module.exports = {
  root: true,
  env: {
    browser: true,
    node: true,
    jest: true,
  },
  parserOptions: {
    sourceType: 'module',
    parser: '@babel/eslint-parser',
  },
  extends: [
    'eslint:recommended',
    // https://github.com/vuejs/eslint-plugin-vue#priority-a-essential-error-prevention
    // consider switching to `plugin:vue/strongly-recommended` or `plugin:vue/recommended` for stricter rules.
    'plugin:vue/recommended',
    'airbnb-base',
    'plugin:custom/default',
  ],
  // required to lint *.vue files
  plugins: [
    'vue',
  ],
  // add your custom rules here
  rules: {
    // don't require .vue extension when importing
    'import/extensions': ['error', 'never'],
    'import/no-unresolved': 0,
    'operator-linebreak': 'off',
    'implicit-arrow-linebreak': 'off',
    'no-multiple-empty-lines': 'off',
    'import/no-unassigned-import': 0,
    'no-import-assign': 'off',
    'vue/multi-word-component-names': 'off',
    'vue/no-computed-properties-in-data': 'off',
    'vue/no-v-text-v-html-on-component': 'off',
    'vue/no-lone-template': 'off',
    'vue/no-mutating-props': 'off',
    'func-names': 'off',
    'vue/no-useless-template-attributes': 'off',
    'import/order': 'off',
    'import/extensions': 'off',
    'default-param-last': 'off',
    'max-len': 'off',
    'prefer-regex-literals': 'off',
    'no-self-assign': 'off',
    'no-dupe-keys': 'off',
    'arrow-parens': 'off',
    'vue/first-attribute-linebreak': 'off',
    'prefer-object-spread': 'off',
    'function-paren-newline': 'off',
    // don't require linebreak before/after opening/closing tags on single or multiline elements
    'vue/singleline-html-element-content-newline': 'off',
    'vue/multiline-html-element-content-newline': 'off',
    'vue/no-unused-components': 'error',
    'vue/no-multi-spaces': 'error',
    // don't require new line after closing bracket
    'vue/html-closing-bracket-newline': 'off',
    // don't require space before closing bracket
    'vue/html-closing-bracket-spacing': 'off',
    'vue/attributes-order': 'error',
    'vue/html-indent': 'error',
    // disallow reassignment of function parameters
    // disallow parameter object manipulation except for specific exclusions
    'no-param-reassign': ['error', {
      props: true,
      ignorePropertyModificationsFor: [
        'state', // for vuex state
        'acc', // for reduce accumulators
        'e', // for e.returnvalue
      ],
    }],
    'object-curly-newline': 'off', // ES6 dereferencing often uses curly braces on a single line.
    'vue/max-attributes-per-line': 'off',
    // allow optionalDependencies
    'import/no-extraneous-dependencies': ['error', {
      optionalDependencies: ['test/unit/index.js'],
    }],
    // allow debugger during development
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    'no-console': 'error',
  },
  settings: {
    'imports/resolver': {
      node: {
        extensions: ['.js', '.vue'],
      },
    },
  },

};
