<template>
  <div v-show="showSpinner">
    <img id="loading-spinner" src="/static/images/spinner.gif" alt="loading" />
  </div>
</template>

<style scoped>
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
  data() {
    return {
      showSpinner: false,
    };
  },
  mounted() {
    this.$root.$on('show-loading-spinner', () => {
      this.showSpinner = true;
    });
    this.$root.$on('hide-loading-spinner', () => {
      this.showSpinner = false;
    });
  },
};
</script>
