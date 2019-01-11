<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div>
      <div :class="$style.info">
        <h2>{{ $t('organDonation.register.subheader') }}</h2>      
      </div>
      <div :class="$style['grid-container']">
        <organ-donation-button id="yes-button" :decision="noDecision"/>
        <organ-donation-button id="no-button" :decision="yesDecision"/>
      </div>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import {
  DECISION_OPT_OUT,
  DECISION_OPT_IN,
} from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    GenericButton,
    OrganDonationButton,
  },
  async asyncData({ store }) {
    await store.dispatch('organDonation/getRegistration');
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
@import "../../style/buttons";
@import "../../style/info";

.info {
  p {
    color: $dark_grey;
    font-weight: bold;
  }
}

.grid-container {
  display: grid;
  grid-template-columns: auto auto;
  grid-gap: 5px;
}
</style>
