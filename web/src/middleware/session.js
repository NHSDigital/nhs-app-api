export default async ({ store }) => {
  if (!store.getters['session/isLoggedIn']()) return Promise.resolve();
  if (store.state.session.hasLoaded) return Promise.resolve();

  await store.dispatch('session/getSession');

  return Promise.resolve();
};
