import get from 'lodash/fp/get';
import {
  INDEX,
  INTERSTITIAL_REDIRECTOR,
  LOGOUT,
  REDIRECT_PARAMETER,
  TERMSANDCONDITIONS,
  findByName,
  isAnonymous,
} from '@/lib/routes';

export default async ({ redirect, route, store }) => {
  if (isAnonymous(route.name)) {
    return Promise.resolve();
  }
  await store.dispatch('termsAndConditions/checkAcceptance');

  if (store.state.termsAndConditions.areAccepted
      && !store.state.termsAndConditions.updatedConsentRequired) {
    if (route.name === TERMSANDCONDITIONS.name) {
      const redirectName = get(REDIRECT_PARAMETER)(route.query);
      const internalRedirect = findByName(redirectName);
      if (internalRedirect) {
        return redirect(internalRedirect.path);
      }
      const path = redirectName ? INTERSTITIAL_REDIRECTOR.path : INDEX.path;
      return redirect(path, route.query);
    }
    return Promise.resolve();
  }

  switch (route.name) {
    case TERMSANDCONDITIONS.name:
    case LOGOUT.name:
      return Promise.resolve();
    default:
      return redirect(TERMSANDCONDITIONS.path, route.query);
  }
};
