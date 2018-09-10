import { isAnonymous } from '@/lib/routes';

const isAcceptedInCookie = ({ app } = {}) => {
  const { $cookies } = app;
  if (!$cookies.get) return false;

  const cookie = $cookies.get('nhso.session');
  return cookie && cookie.termsAccepted;
};

export default async ({ redirect, route, store }) => {
  if (
    isAnonymous(route) ||
    !store.getters['session/isLoggedIn']() ||
    store.state.termsAndConditions.areAccepted
  ) return Promise.resolve();

  if (isAcceptedInCookie(store)) {
    store.dispatch('termsAndConditions/setAcceptance', true);
    return Promise.resolve();
  }

  await store.dispatch('termsAndConditions/checkAcceptance');
  if (store.state.termsAndConditions.areAccepted) return Promise.resolve();
  redirect('/terms-and-conditions');
  return undefined;
};
