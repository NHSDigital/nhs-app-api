<template>
  <div/>
</template>

<script>
import NativeCallbacks from '@/services/native-app';

export default {
  mounted() {
    this.version050compatibility();
    this.$store.dispatch('auth/logout');
  },
  methods: {
    version050compatibility() {
      if (this.$store.state.device.isNativeApp && this.$store.getters['appVersion/isPreForceUpdate']) {
        NativeCallbacks.hideHeader();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
</style>
