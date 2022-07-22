<template>
  <div v-if="isVisible"
       class="loading-spinner-background"
       tabindex="-1"
       focusable="false">
    <div class="loading-spinner"
         tabindex="-1"
         focusable="false">
      <span class="sr-only"
            aria-live="polite">
        {{ spinnerText }}
      </span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Spinner',
  props: {
    alwaysShow: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  computed: {
    isVisible() {
      return this.alwaysShow || (!this.$store.state.spinner.prevent &&
        (this.$store.getters['http/isLoading']
          || this.$store.state.onlineConsultations.isLoadingFile
          || this.$store.state.http.isLoadingExternalSite
          || this.$store.state.biometrics.showBiometricSpinner));
    },
    spinnerText() {
      return this.isVisible ? 'Loading' : '';
    },
  },
};
</script>

<style lang="scss" scoped>
  @import "@/style/custom/spinner";
  @import "../../style/accessibility";
</style>
