<template>
  <div />
</template>

<script>
export default {
  name: '',
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
};
</script>
