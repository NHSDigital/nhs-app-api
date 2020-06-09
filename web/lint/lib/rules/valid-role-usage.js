const utils = require('eslint-plugin-vue/lib/utils');
const { transform } = require('lodash');

module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: 'Should not use role as flagged in accessibility report',
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return utils.defineTemplateBodyVisitor(context, {
      VElement(node) {
        let message;

        if (utils.hasAttribute(node, 'role')) {
          const targetAttribute = utils.getAttribute(node, 'role');
          if (targetAttribute.value && targetAttribute.value.value === 'text') {
              message = 'Should not use role=text, as flagged in accessibility report';
          }
        }
        if (message) {
          context.report({
            node: node.startTag,
            loc: node.startTag.loc,
            message,
          });
        }
      },
    });
  },
};
