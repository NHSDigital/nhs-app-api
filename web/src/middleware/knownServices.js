export default async ({ store, next }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];
  if (isLoggedIn() && !store.state.knownServices.isLoaded) {
    await store.dispatch('knownServices/load');
  }
  return next();
};
