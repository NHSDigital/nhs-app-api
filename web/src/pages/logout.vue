<template>
  <div/>
</template>

<script>
import NativeCallbacks from '@/services/native-app';

export default {
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
    }
    this.$store.dispatch('auth/logout');
  },
};
</script>

<style module lang="scss" scoped>
</style>
