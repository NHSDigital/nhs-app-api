<template>
  <!--eslint-disable vue/html-self-closing -->
  <div v-if="isVisible" class="loading-spinner-background" tabindex="-1" focusable="false">
    <div class="loading-spinner" tabindex="-1" focusable="false"></div>
  </div>
</template>

<script>
export default {
  name: 'Spinner',
  computed: {
    isVisible() {
      return !this.$store.state.spinner.prevent &&
        (this.$store.getters['http/isLoading']
        || this.$store.state.onlineConsultations.isLoadingFile
        || this.$store.state.http.isLoadingExternalSite);
    },
  },
};
</script>

<style lang="scss" scoped>
  @import "@/style/custom/spinner";
</style>
