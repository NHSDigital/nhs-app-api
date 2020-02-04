export default async ({ store }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];
  if (isLoggedIn() && !store.state.knownServices.isLoaded) {
    await store.dispatch('knownServices/load');
  }
};
