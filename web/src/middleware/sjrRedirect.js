import has from 'lodash/fp/has';
import sjrIf from '@/lib/sjrIf';
import { findByName } from '@/lib/routes';

export default ({ redirect, route, store }) => {
  const routeDetail = findByName(route.name);

  if (routeDetail && routeDetail.sjrRedirectRules) {
    for (let i = 0; i < routeDetail.sjrRedirectRules.length; i += 1) {
      const rule = routeDetail.sjrRedirectRules[i];
      const disabled = has('journey_disabled')(rule);
      const journey = disabled ? rule.journey_disabled : rule.journey;

      if (sjrIf({ $store: store, journey, disabled, context: rule.context })) {
        if (routeDetail.path === rule.url) {
          break;
        }
        redirect('302', rule.url);
        break;
      }
    }
  }
};
