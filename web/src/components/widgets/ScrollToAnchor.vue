<template>
  <a :class="classes" />
</template>

<script>
import vueScrollTo from 'vue-scrollto';

export default {
  name: 'ScrollToAnchor',
  computed: {
    classes() {
      return [
        this.$style.scrollToAnchorPosition,
        !this.$store.state.device.isNativeApp || this.$style.scrollToAnchorNativePosition,
      ];
    },
  },
  mounted() {
    vueScrollTo.scrollTo(this.$el, 250, { easing: vueScrollTo['ease-in'] });
  },
};
</script>

<style module lang='scss' scoped>
  @import "@/style/custom/scroll-to-anchor";
</style>
