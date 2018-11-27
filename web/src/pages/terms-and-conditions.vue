<template>
  <div v-if="showTemplate" :class="[$style.webHeader, 'pull-content']">
    <header-slim :show-in-native="true"> {{ pageHeader }} </header-slim>
    <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
    <terms-conditions v-else/>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import HeaderSlim from '@/components/HeaderSlim';
import TermsConditions from '@/components/TermsConditions';
import NativeCallbacks from '@/services/native-app';
import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';

export default {
  layout: 'termsAndConditions',
  components: {
    HeaderSlim,
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
      this.$store.dispatch('pageTitle/updatePageTitle', this.$t('updatedTermsAndConditions.title'));
      if (process.client) {
        window.document.title = `${this.$t('updatedTermsAndConditions.title')} - ${this.$t('appTitle')}`;
      }
    }
    this.version050compatibility();
  },
  methods: {
    version050compatibility() {
      if (this.$store.state.device.isNativeApp && this.$store.getters['appVersion/isPreForceUpdate']) {
        NativeCallbacks.hideHeader();
        NativeCallbacks.hideWhiteScreen();
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
  .webHeader {
    padding: 3.625em 0em 3.125em 2.0px;
  }

</style>
