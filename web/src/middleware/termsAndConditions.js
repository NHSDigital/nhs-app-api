import get from 'lodash/fp/get';
import {
  NOTIFICATIONS_NAME,
  INTERSTITIAL_REDIRECTOR_NAME,
  LOGOUT_NAME,
  REDIRECT_PARAMETER,
  TERMSANDCONDITIONS_NAME,
  isNhsAppRouteName,
} from '@/router/names';
import { isAnonymous } from '@/router';
import { createRouteByNameObject } from '@/lib/utils';

export default async (context) => {
  const { to, store, next } = context;

  if (isAnonymous(to)) {
    return next();
  }

  await store.dispatch('termsAndConditions/checkAcceptance');

  if (store.state.termsAndConditions.areAccepted
      && !store.state.termsAndConditions.updatedConsentRequired) {
    if (to.name === TERMSANDCONDITIONS_NAME) {
      const redirectName = get(REDIRECT_PARAMETER)(to.query);
      if (isNhsAppRouteName(redirectName)) {
        delete to.query[REDIRECT_PARAMETER];
        return next(createRouteByNameObject({
          name: redirectName,
          query: to.query,
          params: to.params,
          store,
        }));
      }
      const name = redirectName ? INTERSTITIAL_REDIRECTOR_NAME : NOTIFICATIONS_NAME;
      return next(createRouteByNameObject({ name, query: to.query, params: to.params, store }));
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
