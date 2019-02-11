<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="showConflictedDecisionFound">
      <message-dialog :icon-text="$t('organDonation.viewDecision.conflictedState.dialogText')"
                      message-id="success-dialog" message-type="success">
        <message-text>
          {{ $t('organDonation.viewDecision.conflictedState.messageText') }}</message-text>
      </message-dialog>
      <strong>{{ $t('organDonation.viewDecision.conflictedState.registrationHeader') }}</strong>
      <p :class="$style.messageText">
        {{ $t('organDonation.viewDecision.conflictedState.registrationText') }}</p>
    </div>
    <div v-else>
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
    <div v-if="hasAppointedRep" :class="[$style.info, $style.appointedRep]">
      <p>{{ $t('organDonation.registered.appointedRep.phoneLabel') }}</p>
      <span>0300 123 2323</span>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
  STATE_CONFLICTED,
} from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    DecisionDetails,
    GenericButton,
    MessageText,
    MessageDialog,
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
    hasAllOrgans() {
      return !!(
        this.hasExistingDecision &&
        get('$store.state.organDonation.originalRegistration.decisionDetails.all')(this)
      );
    },
    hasAppointedRep() {
      return this.decision === DECISION_APPOINTED_REP;
    },
    hasExistingDecision() {
      return this.decision !== DECISION_UNKNOWN;
    },
    hasExistingOptIn() {
      return this.decision === this.yesDecision;
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
    showConflictedDecisionFound() {
      return this.state === STATE_CONFLICTED && this.decision === DECISION_UNKNOWN;
    },
    state() {
      return this.$store.state.organDonation.originalRegistration.state;
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
  &.appointedRep {
    p {
      padding-bottom: 0;
    }
  }
}

.flexbox-container {
  display: flex;
}

.divider {
  margin: 5px;
}
</style>
