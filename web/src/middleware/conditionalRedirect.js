import { getOr, isArray } from 'lodash/fp';
import { findByName } from '@/lib/routes';

export const conditionalRedirector = ({ redirect, path, redirectRules, store }) => {
  if (isArray(redirectRules)) {
    for (let i = 0; i < redirectRules.length; i += 1) {
      const rule = redirectRules[i];
      if (getOr(true, 'value', rule) === store.getters[rule.condition]) {
        if (path === rule.url) {
          break;
        }
        redirect('302', rule.url);
        break;
      }
    }
  }
};

export default ({ redirect, route, store }) => {
  const routeDetail = findByName(route.name);

  if (routeDetail) {
    conditionalRedirector({
      redirect,
      path: routeDetail.path,
      redirectRules: routeDetail.redirectRules,
      store,
    });
  }
};
