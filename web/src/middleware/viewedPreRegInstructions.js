import { BEGINLOGIN_NAME } from '@/router/names';
import { createRouteByNameObject } from '@/lib/utils';

export default (context) => {
  const { to, store, next } = context;

  store.dispatch('preRegistrationInformation/sync');

  if (!store.state.device.isNativeApp || store.state.preRegistrationInformation.seen) {
    const beginLoginRoute = createRouteByNameObject({
      name: BEGINLOGIN_NAME,
      query: to.query,
      params: to.params,
      store,
    });

    return next(beginLoginRoute);
  }

  return next();
};
