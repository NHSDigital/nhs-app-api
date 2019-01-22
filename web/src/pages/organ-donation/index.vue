<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div>
      <div :class="$style.info">
        <h2>{{ $t('organDonation.register.subheader') }}</h2>
      </div>
      <div :class="$style['flexbox-container']">
        <organ-donation-button id="yes-button" :decision="noDecision"/>
        <div :class="$style['divider']"/>
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
  created() {
    this.$store.dispatch('organDonation/setAdditionalDetails', { ethnicityId: '', religionId: '' });
    this.$store.dispatch('organDonation/resetAcceptanceChecks');
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

.flexbox-container {
  display: flex;
}

.divider {
  margin: 5px;
}
</style>
