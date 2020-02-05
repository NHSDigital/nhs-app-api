const isFullLodashImport = str => /^lodash(-es)?$/.test(str);

module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: 'enforce specific lodash imports',
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return {
      ImportDeclaration(node) {
        if (isFullLodashImport(node.source.value)) {
          context.report({
            node,
            message: 'Do not use `import _ from \'lodash\'`. Import packages directly eg. `import map from \'lodash/fp/map\'`' });
        }
      },
    };
  },
};
