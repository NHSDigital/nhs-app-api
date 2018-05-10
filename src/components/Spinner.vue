<template>
  <div v-show="isVisible">
    <img id="loading-spinner" src="/static/images/spinner.gif" alt="loading" />
  </div>
</template>

<style lang="scss" scoped>
#loading-spinner {
  position: absolute;
  left: 0;
  right: 0;
  top: 0;
  bottom: 0;
  margin: auto;
}
</style>

<script>
export default {
  computed: {
    isVisible() {
      return this.$store.state.http.isLoading;
    },
  },
};
</script>
