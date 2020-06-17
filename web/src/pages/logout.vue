<template>
  <div />
</template>

<script>
export default {
  metaInfo() {
    return {
      noscript: [
        { innerHTML: '<meta http-equiv="refresh" content="0;URL=\'/\'">', body: false },
      ],
      __dangerouslyDisableSanitizers: ['noscript'],
    };
  },
  created() {
    this.$store.dispatch('auth/logoutNoJs');
  },
  mounted() {
    sessionStorage.clear();
    this.$store.dispatch('auth/logout');
  },
};
</script>
