
import sjrIf from '@/lib/sjrIf';
import { has } from 'lodash/fp';
import { createRoutePathObject } from '@/lib/utils';

export default ({ next, to, store }) => {
  if (to.meta.sjrRedirectRules) {
    const { sjrRedirectRules } = to.meta;

    for (let i = 0, max = sjrRedirectRules.length; i < max; i += 1) {
      const rule = sjrRedirectRules[i];
      const disabled = has('journey_disabled')(rule);
      const journey = disabled ? rule.journey_disabled : rule.journey;

      if (sjrIf({ $store: store, journey, disabled, context: rule.context })) {
        if (to.name === rule.name) {
          break;
        }
        return next(createRoutePathObject({
          path: rule.url,
          store,
        }));
      }
    }
  }
  return next();
};
