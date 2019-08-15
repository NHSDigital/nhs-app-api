import getOr from 'lodash/fp/getOr';
import { findByName } from '@/lib/routes';

export default ({ redirect, route, store }) => {
  const routeDetail = findByName(route.name);

  if (routeDetail && routeDetail.redirectRules) {
    /* eslint-disable no-restricted-syntax */
    for (const rule of routeDetail.redirectRules) {
      if (getOr(true, 'value', rule) === store.getters[rule.condition]) {
        if (routeDetail.path === rule.url) {
          break;
        }
        redirect('302', rule.url);
        break;
      }
    }
  }
};
