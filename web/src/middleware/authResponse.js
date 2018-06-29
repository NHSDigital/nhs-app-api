/* eslint-disable consistent-return */
export default function (context) {
  if (process.server) {
    return context.store.dispatch('auth/handleAuthResponse', {
      code: context.route.query.code,
    });
  }
}
