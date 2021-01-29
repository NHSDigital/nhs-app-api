const utils = require('eslint-plugin-vue/lib/utils');

const name = 'sjr-if';

module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: 'enforce valid sematic meanings of tags',
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return utils.defineTemplateBodyVisitor(context, {
      'VElement[name=\'b\']': (node) => {
        const message = '\'<b>\' tags do not have a sematic meaning. Please use a header tag, or a strong tag.';

        context.report({
          node: node.startTag,
          loc: node.startTag.loc,
          message,
          data: { name },
        });
      },
    });
  },
};
