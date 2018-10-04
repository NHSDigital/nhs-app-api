import { isAnonymous } from '@/lib/routes';

export default async ({ redirect, route, store }) => {
  if (isAnonymous(route.name)) {
    return Promise.resolve();
  }
  await store.dispatch('termsAndConditions/checkAcceptance');
  if (store.state.termsAndConditions.areAccepted) {
    if (route.name === 'terms-and-conditions') {
      return redirect('/');
    }
    return Promise.resolve();
  }
  if (route.name === 'terms-and-conditions') {
    return Promise.resolve();
  }
  return redirect('/terms-and-conditions');
};
