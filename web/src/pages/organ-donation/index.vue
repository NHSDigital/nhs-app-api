<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="hasExistingDecision">
      <your-decision :decision="decision"
                     :decision-details="decisionDetails"
                     header-key="organDonation.registered.yourDecision.subheader"/>
      <div v-if="hasExistingOptIn">
        <decision-details
          v-if="hasSomeOrgans"
          :choices="choices"/>
      </div>
    </div>
    <div v-else>
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
import get from 'lodash/fp/get';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import GenericButton from '@/components/widgets/GenericButton';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  DECISION_NOT_FOUND,
  DECISION_OPT_OUT,
  DECISION_OPT_IN,
} from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    DecisionDetails,
    GenericButton,
    OrganDonationButton,
    YourDecision,
  },
  async asyncData({ store }) {
    await store.dispatch('organDonation/getRegistration');
  },
  computed: {
    choices() {
      return get('$store.state.organDonation.originalRegistration.decisionDetails.choices')(this);
    },
    decision() {
      return this.$store.state.organDonation.originalRegistration.decision;
    },
    decisionDetails() {
      return this.$store.state.organDonation.originalRegistration.decisionDetails;
    },
    hasExistingDecision() {
      return this.$store.state.organDonation.originalRegistration.decision !== DECISION_NOT_FOUND;
    },
    hasAllOrgans() {
      return !!(
        this.hasExistingDecision &&
        get('$store.state.organDonation.originalRegistration.decisionDetails.all')(this)
      );
    },
    hasExistingOptIn() {
      return this.$store.state.organDonation.originalRegistration.decision === DECISION_OPT_IN;
    },
    hasSomeOrgans() {
      return !!(
        this.hasExistingDecision &&
        get('$store.state.organDonation.originalRegistration.decisionDetails.all')(this) === false
      );
    },
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
