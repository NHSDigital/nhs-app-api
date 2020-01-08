<template>
  <div class="nhsuk-u-padding-bottom-4">
    <h2>{{ headerText }}</h2>
    <div class="nhsuk-grid-row" :class="$style['flex-container']">
      <organ-donation-button id="no-button"
                             class="nhsuk-grid-column-one-half
                             nhsuk-u-padding-0
                             nhsuk-u-margin-4
                             nhsuk-u-one-half choiceButton"
                             :decision="noDecision"/>
      <organ-donation-button id="yes-button"
                             class="nhsuk-grid-column-one-half
                             nhsuk-u-padding-0
                             nhsuk-u-margin-4
                             nhsuk-u-one-half choiceButton"
                             :decision="yesDecision"/>
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
.flex-container{
  display: flex;
}
</style>

<style>
  .nhsuk-u-one-half.choiceButton{
    background: #fff;
  }
</style>
