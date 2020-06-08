const utils = require('eslint-plugin-vue/lib/utils');
const { getterNames } = require('../../.transpiled/constants');

const name = 'sjr-if';
const getterNameValues = Object.values(getterNames);

module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: `enforce valid sematic meanings of tags`,
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return utils.defineTemplateBodyVisitor(context, {
      [`VElement[name='b']`]: (node) => {
        let message= `'<b>' tags do not have a sematic meaning. Please use a header tag, or a strong tag.`;

        context.report({
          node: node.startTag,
          loc: node.startTag.loc,
          message,
          data: { name },
        });
      }
    });
  },
};
