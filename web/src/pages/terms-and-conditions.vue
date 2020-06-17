<template>
  <terms-and-conditions-layout>
    <div :class="$store.state.device.isNativeApp && 'pull-content'">
      <updated-terms-conditions v-if="isUpdatedConsentRequired"/>
      <terms-conditions v-else/>
    </div>
  </terms-and-conditions-layout>
</template>
<script>
import TermsConditions from '@/components/TermsConditions';
import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';
import TermsAndConditionsLayout from '@/layouts/termsAndConditions';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';

export default {
  name: 'TermsAndConditionsPage',
  components: {
    TermsConditions,
    UpdatedTermsConditions,
    TermsAndConditionsLayout,
  },
  data() {
    return {
      areAccepted: this.$store.state.termsAndConditions.areAccepted,
      updatedConsentRequired: this.$store.state.termsAndConditions.updatedConsentRequired,
    };
  },
  computed: {
    pageHeader() {
      return this.isUpdatedConsentRequired
        ? 'updatedTermsAndConditions.title'
        : 'termsAndConditions.title';
    },
    isUpdatedConsentRequired() {
      return this.areAccepted && this.updatedConsentRequired;
    },
  },
  beforeMount() {
    EventBus.$emit(UPDATE_HEADER, this.pageHeader);
  },
  mounted() {
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
