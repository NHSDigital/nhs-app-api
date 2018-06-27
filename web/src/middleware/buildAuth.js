export default ({ store }) => {
  if (process.server) {
    store.dispatch('auth/buildLogin');
  }
};
