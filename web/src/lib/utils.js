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
export const readableBytes = (bytes) => {
  if (Number.isNaN(Number(bytes)) || bytes < 0) {
    return bytes;
  }

  if (bytes < 1000) {
    const convertedBytes = Math.round(bytes / 1);
    if (convertedBytes === bytes) {
      return `${convertedBytes}B`;
    }
    return readableBytes(convertedBytes);
  }

  if (bytes < 1000000) {
    const convertedBytes = Math.round(bytes / 1000);
    if (convertedBytes * 1000 === bytes) {
      return `${convertedBytes}KB`;
    }
    return readableBytes(convertedBytes * 1000);
  }

  return `${Number(parseFloat(bytes / 1000000).toFixed(2))}MB`;
};
