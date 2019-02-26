export const isFalsy = value => !(value && value !== 'false');
export const isTruthy = value => !isFalsy(value);
export const redirectTo = (self, path, query) => {
  if (process.server) {
    if (query == null) {
      self.$store.app.context.redirect(path);
    } else {
      self.$store.app.context.redirect(302, path, query);
    }
  } else if (query == null) {
    self.$router.push(path);
  } else {
    self.$router.push({ path, query });
  }
};
