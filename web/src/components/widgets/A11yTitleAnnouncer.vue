<template>
  <div :class="$style['sr-only']" role="presentation">
    <span aria-live="polite">{{ $store.state.pageTitle.pageTitle + ' page' }}</span>
  </div>
</template>

<script>

export default {
  data() {
    return {
      a11yText: `${this.$store.state.pageTitle.pageTitle}`,
      delayTimeoutId: undefined,
    };
  },
  methods: {
    announceHeader() {
      if (this.delayTimeoutId) {
        clearTimeout(this.delayTimeoutId);
      }
      this.delayTimeoutId = setTimeout(() => {
        this.a11yText = this.$store.state.pageTitle.pageTitle;
      }, 1000);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/accessibility";
</style>
