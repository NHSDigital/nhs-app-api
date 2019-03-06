<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
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
    <div v-else-if="hasExistingDecision">
      <your-decision :decision="decision"
                     :decision-details="decisionDetails"
                     header-key="organDonation.registered.yourDecision.subheader"/>
      <div v-if="hasExistingOptIn">
        <decision-details
          v-if="hasSomeOrgans"
          :choices="choices"/>
      </div>
      <amend-decision-link :class="$style.amendDecision"/>
      <next-steps v-if="hasExistingOptIn || hasExistingOptOut"
                  :is-opt-in-decision="hasExistingOptIn"/>
    </div>
    <div v-else>
      <make-decision/>
      <ul :class="$style['list-menu']">
        <li>
          <already-registered-link/>
        </li>
        <li>
          <find-out-more-link/>
        </li>
      </ul>
    </div>
    <div v-if="hasAppointedRep" :class="[$style.info, $style.appointedRep]">
      <p>{{ $t('organDonation.registered.appointedRep.phoneLabel') }}</p>
      <span>0300 123 2323</span>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import AlreadyRegisteredLink from '@/components/organ-donation/AlreadyRegisteredLink';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NextSteps from '@/components/organ-donation/NextSteps';
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
    AmendDecisionLink,
    DecisionDetails,
    AlreadyRegisteredLink,
    FindOutMoreLink,
    MakeDecision,
    MessageText,
    MessageDialog,
    NextSteps,
    YourDecision,
  },
  async asyncData({ store }) {
    await store.dispatch('organDonation/getReferenceData');
    await store.dispatch('organDonation/getRegistration');
  },
  computed: {
    choices() {
      return get('choices')(this.decisionDetails);
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
        get('all')(this.decisionDetails)
      );
    },
    hasAppointedRep() {
      return this.decision === DECISION_APPOINTED_REP;
    },
    hasExistingDecision() {
      return this.decision !== DECISION_UNKNOWN;
    },
    hasExistingOptIn() {
      return this.decision === DECISION_OPT_IN;
    },
    hasExistingOptOut() {
      return this.decision === DECISION_OPT_OUT;
    },
    hasSomeOrgans() {
      return !!(
        this.hasExistingDecision &&
        get('all')(this.decisionDetails) === false
      );
    },
    showConflictedDecisionFound() {
      return this.state === STATE_CONFLICTED && this.decision === DECISION_UNKNOWN;
    },
    state() {
      return this.$store.state.organDonation.originalRegistration.state;
    },
  },
  created() {
    this.$store.dispatch('organDonation/amendCancel');
    this.$store.dispatch('organDonation/setAdditionalDetails', { ethnicityId: '', religionId: '' });
    this.$store.dispatch('organDonation/resetAcceptanceChecks');
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/listmenu";
@import "../../style/spacings";

.amendDecision {
  margin-bottom: $three;
}

.info {
  &.appointedRep {
    p {
      padding-bottom: 0;
    }
  }
}

.list-menu {
  margin-bottom: $three;
}
</style>
