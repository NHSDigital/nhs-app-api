<template>
  <div :class="$style['mb-3']">
    <div :class="$style.info">
      <h2>{{ headerText }}</h2>
    </div>
    <div :class="$style['flexbox-container']">
      <organ-donation-button id="no-button" :decision="noDecision"/>
      <div :class="$style['m-1']"/>
      <organ-donation-button id="yes-button" :decision="yesDecision"/>
    </div>
  </div>
</template>

<script>
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import { DECISION_OPT_IN, DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'MakeDecision',
  components: {
    OrganDonationButton,
  },
  data() {
    return {
      headerText: this.$store.state.organDonation.isAmending
        ? this.$t('organDonation.register.subheaderAmend')
        : this.$t('organDonation.register.subheaderRegister'),
      noDecision: DECISION_OPT_OUT,
      yesDecision: DECISION_OPT_IN,
    };
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/spacings";

.flexbox-container {
  display: flex;
}
</style>
