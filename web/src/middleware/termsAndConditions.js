import { LOGOUT, INDEX, TERMSANDCONDITIONS, isAnonymous } from '@/lib/routes';

export default async ({ redirect, route, store }) => {
  if (isAnonymous(route.name)) {
    return Promise.resolve();
  }
  await store.dispatch('termsAndConditions/checkAcceptance');

  if ((store.state.termsAndConditions.areAccepted)
      && (!store.state.termsAndConditions.updatedConsentRequired)) {
    if (route.name === TERMSANDCONDITIONS.name) {
      return redirect(INDEX.path);
    }
    return Promise.resolve();
  }

  switch (route.name) {
    case TERMSANDCONDITIONS.name:
    case LOGOUT.name:
      return Promise.resolve();
    default:
      return redirect(TERMSANDCONDITIONS.path);
  }
};
