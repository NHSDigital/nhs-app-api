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
        if(node.parent.name ==='message-dialog' && !['message-text', 'message-list', 'component'].includes(node.name)) {
          message = '<message-dialog> must have <message-x> children, but was ' + node.name;
        }
        if(node.parent !=null) {
          if(node.parent.name ==='message-list' && node.name != 'li') {
            message = '<message-list> must have <li> children, but was ' + node.name;
          }
          if(node.parent.parent !=null && node.parent.parent.name ==='message-list' && node.parent.name === 'li') {
            message = '<message-list> must have <li> children, and no grandchildren, but was ' + node.name;
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
