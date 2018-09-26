<template>
  <div/>
</template>

<script>
export default {
  mounted() {
    this.$store.dispatch('auth/logout');
  },
};
</script>

<style module lang="scss" scoped>
</style>
