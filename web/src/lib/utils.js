import { isUndefined, isEqual } from 'lodash/fp';

export const isFalsy = value => !(value && value !== 'false');
export const isTruthy = value => !isFalsy(value);
export const redirectTo = (self, path, query) => {
  if (process.server) {
    if (!query) {
      self.$store.app.context.redirect(path);
    } else {
      self.$store.app.context.redirect(302, path, query);
    }
  } else if (self.$router.currentRoute && self.$router.currentRoute.path === path) {
    const localQuery = (isUndefined(query) && self.$store.state.device.isNativeApp)
      ? {
        ...self.$router.currentRoute.query,
        ...{ source: self.$store.state.device.source },
      }
      : query;

    if (!localQuery || isEqual(self.$router.currentRoute.query, localQuery)) {
      self.$router.go();
    } else {
      self.$router.push({ path, query: localQuery });
    }
  } else if (!query) {
    self.$router.push(path);
  } else {
    self.$router.push({ path, query });
  }
};
