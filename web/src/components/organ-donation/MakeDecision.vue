<template>
  <div :class="$style.organDonationStart">
    <div :class="$style.info">
      <h2>{{ $t('organDonation.register.subheader') }}</h2>
    </div>
    <div :class="$style['flexbox-container']">
      <organ-donation-button id="yes-button" :decision="noDecision"/>
      <div :class="$style.divider"/>
      <organ-donation-button id="no-button" :decision="yesDecision"/>
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
  computed: {
    noDecision() {
      return DECISION_OPT_OUT;
    },
    yesDecision() {
      return DECISION_OPT_IN;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/spacings";

.divider {
  margin: $one;
}

.flexbox-container {
  display: flex;
}
.organDonationStart {
  margin-bottom: $three;
}
</style>
