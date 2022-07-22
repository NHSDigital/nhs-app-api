import { INDEX_NAME } from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';
import NativeApp from '@/services/native-app';

export default async ({ to, store, next }) => {
  const redirectRoute = createConditionalRedirectRouteByName({
    name: INDEX_NAME,
    query: to.query,
    params: to.params,
    store,
  });

  if (!store.state.device.isNativeApp || !store.$env.BIOMETRICS_REGISTRATION_ENABLED) {
    if (NativeApp.goToLoggedInHomeScreen()) {
      return;
    }

    next(redirectRoute);
    return;
  }

  await store.dispatch('biometrics/checkBiometricsCookie');

  if (store.state.biometrics.biometricsCookieExists) {
    next(redirectRoute);
  }

  next();
};
