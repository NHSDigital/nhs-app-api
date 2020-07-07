<template>
  <div>
    <div :class="$store.state.device.isNativeApp && 'pull-content'">
      <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
      <terms-conditions v-else/>
    </div>
  </div>
</template>
<script>
import TermsConditions from '@/components/TermsConditions';
import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';

export default {
  layout: 'termsAndConditions',
  components: {
    TermsConditions,
    UpdatedTermsConditions,
  },
  data() {
    return {
      areAccepted: this.$store.state.termsAndConditions.areAccepted,
      updatedConsentRequired: this.$store.state.termsAndConditions.updatedConsentRequired,
    };
  },
  computed: {
    pageHeader() {
      if ((this.areAccepted)
        && (this.updatedConsentRequired)) {
        return this.$t('updatedTermsAndConditions.title');
      }
      return this.$t('termsAndConditions.title');
    },
    isUpdatedConsentRequired() {
      return ((this.areAccepted)
        && (this.updatedConsentRequired));
    },
  },
  mounted() {
    if (this.$store.state.termsAndConditions.updatedConsentRequired) {
      this.$store.dispatch('header/updateHeaderText', this.pageHeader);
      if (process.client) {
        window.document.title = `${this.$t('updatedTermsAndConditions.title')} - ${this.$t('appTitle')}`;
      }
    }

    this.$store.dispatch('device/pageLoadComplete');
  },

};
</script>

<style lang="scss">
  @import "../style/main";
  @import "../style/pulltorefresh";
  @import "../style/elements";
</style>

<style module lang="scss" scoped>
  @import "../style/spacings";

  section {
    display: block;
    padding: 0 1em 2.5em;
  }

</style>
