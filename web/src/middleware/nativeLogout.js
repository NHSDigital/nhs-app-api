import NativeApp from '@/services/native-app';

export default ({ router, store, next }) => {
  const showSessionExpiredBanner = store.state.session.showExpiryMessage
    || router.currentRoute.query.showExpiryMessage;

  if (NativeApp.supportsSessionExpired() && showSessionExpiredBanner) {
    NativeApp.sessionExpired();
    return;
  }

  if (NativeApp.supportsLogout()) {
    NativeApp.logout();
    return;
  }

  next();
};
