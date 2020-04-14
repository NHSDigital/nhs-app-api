const utils = require('eslint-plugin-vue/lib/utils');
const { getterNames } = require('../../.transpiled/constants');

const name = 'sjr-if';
const getterNameValues = Object.values(getterNames);

module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: `enforce valid usage of '${name}'`,
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return utils.defineTemplateBodyVisitor(context, {
      [`VElement[name='${name}']`]: (node) => {
        let message;

        if (!utils.hasAttribute(node, 'journey')) {
          message = `'<${name}>' should have a 'journey' attribute.`;
        } else {
          const { value } = utils.getAttribute(node, 'journey');

          if (value === null) {
            message = `'<${name}>' missing 'journey' value.`;
          } else if (!getterNameValues.includes(`${value.value}Enabled`)) {
            message = `'${value.value}' journey does not have a corresponding getter.`;
          }
        }

        if (message) {
          context.report({
            node: node.startTag,
            loc: node.startTag.loc,
            message,
            data: { name },
          });
        }
      },
    });
  },
};
