module.exports = {
  meta: {
    type: 'problem',
    docs: {
      description: 'Optional chaining is not allowed.',
    },
    fixable: null,
    schema: [],
  },
  create(context) {
    return {
      ChainExpression(node) {
        const message = "Pingdom currently doesn't support Optional chaining so lets not use it for now. If you are seeing this make sure and check if that's still the case.";
        context.report({
          node,
          message,
        });
      },
    };
  },
};
