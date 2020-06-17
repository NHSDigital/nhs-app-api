import isEmpty from 'lodash/fp/isEmpty';

export default async ({ store, next }) => {
  if (isEmpty(store.state.auth.config)) {
    await store.dispatch('auth/updateConfig', store.$cookies.get('nhso.auth'));
  }

  if (isEmpty(store.state.session.user)) {
    const sessionCookie = store.$cookies.get('nhso.session');
    if (sessionCookie) {
      await store.dispatch('session/setInfo', sessionCookie);
    }
  }
  return next();
};
