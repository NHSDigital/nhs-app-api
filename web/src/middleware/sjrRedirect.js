import has from 'lodash/fp/has';
import sjrIf from '@/lib/sjrIf';
import { findByName } from '@/lib/routes';

export default ({ redirect, route, store }) => {
  const routeDetail = findByName(route.name);

  if (routeDetail && routeDetail.sjrRedirectRules) {
    /* eslint-disable no-restricted-syntax */
    for (const rule of routeDetail.sjrRedirectRules) {
      const disabled = has('journey_disabled')(rule);
      const journey = disabled ? rule.journey_disabled : rule.journey;

      const hasContext = has('rule_context')(rule);
      const silverIntegrationRedirect = hasContext
        ? !store.getters[`serviceJourneyRules/${journey}Enabled`](rule.rule_context)
        : false;

      if (sjrIf({ $store: store, journey, disabled }) || silverIntegrationRedirect) {
        if (routeDetail.path === rule.url) {
          break;
        }
        redirect('302', rule.url);
        break;
      }
    }
  }
};
