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
  .scrollToAnchorPosition, .scrollToAnchorPosition:hover {
    padding-top: 15px;
    background-color: transparent;
    box-shadow: 0 0 0 0;
  }

  .scrollToAnchorNativePosition, .scrollToAnchorNativePosition:hover  {
    padding-top: 0;
    margin-top:-50px;
    padding-bottom:50px;
  }
</style>
