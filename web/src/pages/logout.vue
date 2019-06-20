<template>
  <div />
</template>

<script>
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
    this.$store.dispatch('auth/logout');
  },
};
</script>

<style module lang="scss" scoped>
</style>
