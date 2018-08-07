/* eslint-disable consistent-return */
export default function (context) {
  if (process.server) {
    return context.store.dispatch('auth/handleAuthResponse', context.route.query.code);
  }
}
