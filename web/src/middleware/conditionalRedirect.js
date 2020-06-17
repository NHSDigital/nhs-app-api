import { getOr, isArray } from 'lodash/fp';
import { isNhsAppRouteName } from '@/router/names';
import { createRoutePathObject } from '@/lib/utils';

export const conditionalRedirector = ({ redirectRules, store, name }) => {
  if (isArray(redirectRules)) {
    for (let i = 0; i < redirectRules.length; i += 1) {
      const rule = redirectRules[i];
      const result = rule.context
        ? store.getters[rule.condition](rule.context)
        : store.getters[rule.condition];

      if (getOr(true, 'value', rule) === result) {
        if (name === rule.route.name) {
          break;
        }
        return createRoutePathObject({ path: rule.route.path, store });
      }
    }
  }
  return false;
};

export default ({ to, store, next }) => {
  const internalRoute = isNhsAppRouteName(to.name);
  let redirect;
  if (internalRoute) {
    redirect = conditionalRedirector({
      next,
      name: to.name,
      redirectRules: to.meta.redirectRules,
      store,
    });
  }

  if (redirect) {
    return next(redirect);
  }
  return next();
};
