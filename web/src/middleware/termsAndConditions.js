export default async ({ redirect, route, store }) => {
  if (route.name === 'terms-and-conditions') {
    await store.dispatch('termsAndConditions/checkAcceptance');
    if (store.state.termsAndConditions.areAccepted) {
      return redirect('/');
    }
  }
  return Promise.resolve();
};
