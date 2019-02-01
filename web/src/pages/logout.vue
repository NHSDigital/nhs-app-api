<template>
  <div />
</template>

<script>
import NativeCallbacks from '@/services/native-app';

export default {
  head() {
    return {
      noscript: [
        { innerHTML: '<meta http-equiv="refresh" content="0;URL=\'/\'">', body: false },
      ],
      title: `${this.$store.state.header.headerText} screen`,
      __dangerouslyDisableSanitizers: ['noscript'],
    };
  },
  created() {
    this.$store.dispatch('auth/logoutNoJs');
  },
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
