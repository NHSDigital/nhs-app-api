export default {
  name: 'ClearBackLinkOverrideMixin',
  watch: {
    $route(to, from) {
      if (from !== to && to.meta.isHub) {
        this.$store.dispatch('navigation/clearBackLinkOverride');
      }
    },
  },
};
