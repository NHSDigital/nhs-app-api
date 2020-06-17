export default async ({ store, next }) => {
  if (!store.getters['session/isLoggedIn']()) return next();
  if (store.state.session.hasLoaded) return next();

  await store.dispatch('session/getSession');

  return next();
};
