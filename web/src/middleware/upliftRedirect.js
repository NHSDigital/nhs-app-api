import { conditionalRedirector } from '@/middleware/conditionalRedirect';

export default ({ to, store, next }) => {
  const { upliftRoute, proofLevel } = to.meta;
  if (proofLevel && upliftRoute) {
    const redirect = conditionalRedirector({
      path: to.path,
      store,
      redirectRules: [{
        condition: 'session/shouldUplift',
        route: upliftRoute,
        context: proofLevel,
      }],
    });
    if (redirect) {
      return next(redirect);
    }
  }
  return next();
};

