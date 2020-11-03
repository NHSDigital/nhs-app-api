import { NOTIFICATIONS_PATH } from '@/router/paths';
import { createRouteByNameObject } from '@/lib/utils';
import { INDEX_NAME } from '@/router/names';

export default async ({ to, store, next }) => {
  const isNativeVersionAfter = store.getters['appVersion/isNativeVersionAfter'];
  const isWebVersionAfter = store.getters['appVersion/isWebVersionAfter'];

  if (store.state.device.isNativeApp
    && (isNativeVersionAfter('1.41.0') || isWebVersionAfter('develop'))
    && to.path === NOTIFICATIONS_PATH) {
    await store.dispatch('notifications/load');
    await store.dispatch('notifications/checkNotificationCookie');

    return next();
  }

  return next(createRouteByNameObject({
    name: INDEX_NAME,
    query: to.query,
    params: to.params,
    store,
  }));
};
