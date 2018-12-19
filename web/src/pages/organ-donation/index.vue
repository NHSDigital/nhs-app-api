<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="hasMadeDecision">
      <div :class="$style.info">
        <p>{{ $t('organDonation.additionalDetails.subheader') }}</p>
      </div>
    </div>
    <div v-else>
      <div :class="$style.info">
        <p>{{ $t('organDonation.register.subheader') }}</p>
      </div>
      <organ-donation-button :decision="noDecision"/>
    </div>
  </div>
</template>

<script>
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import {
  DECISION_NOT_FOUND,
  DECISION_OPT_OUT,
} from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    OrganDonationButton,
  },
  async asyncData({ store }) {
    await store.dispatch('organDonation/getRegistration');
  },
  computed: {
    hasMadeDecision() {
      return this.$store.state.organDonation.registration.decision !== DECISION_NOT_FOUND;
    },
    noDecision() {
      return DECISION_OPT_OUT;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";

.info {
  p {
    color: $dark_grey;
    font-weight: bold;
  }
}
</style>
