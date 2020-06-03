export default {
  name: 'ResetSpinnerMixin',
  watch: {
    $route(to, from) {
      if (from !== to) {
        this.$store.dispatch('spinner/prevent', false);
      }
    },
  },
  created() {
    this.$store.dispatch('spinner/prevent', false);
  },
};
