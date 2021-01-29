import { INDEX_NAME } from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';

export default async ({ to, store, next }) => {
  if (!store.state.device.isNativeApp) {
    const redirectRoute = createConditionalRedirectRouteByName({
      name: INDEX_NAME,
      query: to.query,
      params: to.params,
      store,
    });

    return next(redirectRoute);
  }

  await store.dispatch('notifications/load');
  await store.dispatch('notifications/checkNotificationCookie');

  return next();
};
