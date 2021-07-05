import sjrIf from '@/lib/sjrIf';
import { INDEX_NAME } from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';
import NativeApp from '@/services/native-app';

export default async ({ to, store, next }) => {
  if (!store.state.device.isNativeApp
    || sjrIf({ $store: store, journey: 'notificationPrompt', disabled: true })) {
    if (NativeApp.goToLoggedInHomeScreen()) {
      return;
    }
    const redirectRoute = createConditionalRedirectRouteByName({
      name: INDEX_NAME,
      query: to.query,
      params: to.params,
      store,
    });

    next(redirectRoute);
    return;
  }

  await store.dispatch('notifications/checkNotificationCookie');

  if (!store.state.notifications.notificationCookieExists) {
    await store.dispatch('notifications/load');
  }

  next();
};
