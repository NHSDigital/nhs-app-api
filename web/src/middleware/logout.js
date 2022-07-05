import { createRouteByNameObject } from '@/lib/utils';
import { INDEX_NAME } from '@/router/names';

export default ({ to, store, next }) => {
  const isLoggedIn = store.getters['session/isLoggedIn'];
  const hasActionedLogout = store.getters['session/hasActionedLogout'];
  if (isLoggedIn && !hasActionedLogout) {
    next(createRouteByNameObject({
      name: INDEX_NAME,
      query: to.query,
      params: to.params,
      store,
    }));
    return;
  }
  next();
};
