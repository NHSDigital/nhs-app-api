import { NOTIFICATIONS_PATH } from '@/router/paths';

export default async ({ to, store, next }) => {
  if (store.state.device.isNativeApp && to.path === NOTIFICATIONS_PATH) {
    await store.dispatch('notifications/load');
    await store.dispatch('notifications/checkNotificationCookie');
  }
  next();
};
