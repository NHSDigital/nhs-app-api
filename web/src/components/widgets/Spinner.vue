<template>
  <div v-if="isVisible">
    <img id="loading-spinner" src="~assets/spinnerImg.gif" alt="loading">
  </div>
</template>

<script>
export default {
  computed: {
    isVisible() {
      return this.$store.state.http.loadingCounter;
    },
  },
};
</script>

<style lang="scss" scoped>#loading-spinner {
  position: fixed;
  z-index: 10;
  top: 0em;
  right: 0em;
  left: 0em;
  bottom: 0em;
  margin: auto;
}

</style>
