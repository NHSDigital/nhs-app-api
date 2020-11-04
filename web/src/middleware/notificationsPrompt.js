import { NOTIFICATIONS_PATH } from '@/router/paths';
import { createRouteByNameObject } from '@/lib/utils';
import { INDEX_NAME } from '@/router/names';

export default async ({ to, store, next }) => {
  const isNativeVersionAfter = store.getters['appVersion/isNativeVersionAfter'];
  if (to.path !== NOTIFICATIONS_PATH) {
    return next();
  }

  if (to.path === NOTIFICATIONS_PATH
  && (!isNativeVersionAfter('1.41.0') || !store.state.device.isNativeApp)) {
    return next(createRouteByNameObject({
      name: INDEX_NAME,
      query: to.query,
      params: to.params,
      store,
    }));
  }

  await store.dispatch('notifications/load');
  await store.dispatch('notifications/checkNotificationCookie');

  return next();
};
