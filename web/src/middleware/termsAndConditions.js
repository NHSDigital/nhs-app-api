import {
  NOTIFICATIONS_NAME,
  LOGOUT_NAME,
  TERMSANDCONDITIONS_NAME,
} from '@/router/names';
import { isAnonymous } from '@/router';
import { createConditionalRedirectRouteByName } from '@/lib/utils';

export default async (context) => {
  const { to, store, next } = context;

  if (isAnonymous(to)) {
    return next();
  }

  await store.dispatch('termsAndConditions/checkAcceptance');

  if (store.state.termsAndConditions.areAccepted
      && !store.state.termsAndConditions.updatedConsentRequired) {
    if (to.name === TERMSANDCONDITIONS_NAME) {
      const redirectRoute = createConditionalRedirectRouteByName({
        name: NOTIFICATIONS_NAME,
        query: to.query,
        params: to.params,
        store,
      });

      return next(redirectRoute);
    }
    return next();
  }

  switch (to.name) {
    case TERMSANDCONDITIONS_NAME:
    case LOGOUT_NAME:
      return next();
    default:
      return next({
        name: TERMSANDCONDITIONS_NAME,
        query: to.query,
        params: to.params,
      });
  }
};
