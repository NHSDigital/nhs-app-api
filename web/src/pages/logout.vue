<template>
  <div/>
</template>

<script>
export default {
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      window.nativeApp.hideHeader();
    }
    this.$store.dispatch('auth/logout');
  },
};
</script>

<style module lang="scss" scoped>
</style>
