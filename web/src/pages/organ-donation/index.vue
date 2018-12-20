<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div :class="$style.info">
      <h2>{{ $t('organDonation.register.subheader') }}</h2>
    </div>
    <organ-donation-button :decision="noDecision"/>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import {
  DECISION_OPT_OUT,
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
</style>
