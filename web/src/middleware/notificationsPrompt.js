import { NOTIFICATIONS_PATH } from '@/router/paths';
import { createRouteByNameObject } from '@/lib/utils';
import { INDEX_NAME } from '@/router/names';

const redirectToIndex = (to, store, next) =>
  next(createRouteByNameObject({
    name: INDEX_NAME,
    query: to.query,
    params: to.params,
    store,
  }));

export default async ({ to, store, next }) => {
  if (to.path !== NOTIFICATIONS_PATH) {
    return next();
  }

  if (!store.state.device.isNativeApp) {
    return redirectToIndex(to, store, next);
  }

  const isNativeVersionAfter = store.getters['appVersion/isNativeVersionAfter'];

  if (!isNativeVersionAfter('1.41.x')) {
    // Firebase on android will crash on some devices on pre v1.42 app versions
    // so we want to skip triggering a `load` native method which will trap the
    // user in a 'blue screen of death'; remove after 1.42 release.
    return redirectToIndex(to, store, next);
  }

  await store.dispatch('notifications/load');
  await store.dispatch('notifications/checkNotificationCookie');

  return next();
};
