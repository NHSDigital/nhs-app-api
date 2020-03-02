const utils = require('eslint-plugin-vue/lib/utils');

module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: 'enforce usage of rel="noopener noreferrer" with target="_blank" to prevent tabnabbing',
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return utils.defineTemplateBodyVisitor(context, {
      VElement(node) {
        let message;

        if (utils.hasAttribute(node, 'target')) {
          const targetAttribute = utils.getAttribute(node, 'target');
          if (targetAttribute.value && targetAttribute.value.value === '_blank'
            && targetAttribute.parent && targetAttribute.parent.parent.name === 'a') {
            if (!utils.hasAttribute(node, 'rel')) {
              message = 'Should have a rel attribute with value of "noopener noreferrer"';
            } else {
              const relAttribute = utils.getAttribute(node, 'rel');

              if (relAttribute.value === null || relAttribute.value.value !== 'noopener noreferrer') {
                message = 'rel attribute should have a value of "noopener noreferrer"';
              }
            }
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
