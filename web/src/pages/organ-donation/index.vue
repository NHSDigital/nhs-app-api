<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="isConflicted || hasExistingDecision">
      <div v-if="isConflicted" :class="$style['mb-3']">
        <message-dialog :icon-text="$t('organDonation.viewDecision.conflictedState.dialogText')"
                        message-id="success-dialog" message-type="success">
          <message-text>
            {{ $t('organDonation.viewDecision.conflictedState.messageText') }}</message-text>
        </message-dialog>
        <strong>{{ $t('organDonation.viewDecision.conflictedState.registrationHeader') }}</strong>
        <p :class="$style.messageText">
          {{ $t('organDonation.viewDecision.conflictedState.registrationText') }}</p>
      </div>
      <div v-else :class="$style['mb-3']">
        <your-decision :decision="decision" :decision-details="decisionDetails"
                       header-key="organDonation.registered.yourDecision.subheader"/>
        <div v-if="hasExistingOptIn">
          <decision-details v-if="isSomeOrgans" :choices="choices"/>
          <faith-details-registered :declaration="faithDeclaration"/>
        </div>

        <still-your-decision :is-some-organs="isSomeOrgans"
                             :show-amend="true"
                             :show-reaffirm="!hasAppointedRep"/>

        <div v-if="hasAppointedRep" :class="[$style.info, $style.appointedRep, $style['mt-3']]">
          <p>{{ $t('organDonation.registered.appointedRep.phoneLabel') }}</p>
          <span>0300 123 2323</span>
        </div>
      </div>
      <next-steps v-if="!hasAppointedRep && (hasExistingOptIn || hasExistingOptOut)"
                  :class="$style['mb-3']" :is-opt-in-decision="hasExistingOptIn"/>
      <other-things-to-do :class="$style['mb-3']" :can-withdraw="!isConflicted"/>
    </div>
    <div v-else :class="$style['mb-6']">
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
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AlreadyRegisteredLink from '@/components/organ-donation/AlreadyRegisteredLink';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NextSteps from '@/components/organ-donation/NextSteps';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import StillYourDecision from '@/components/organ-donation/StillYourDecision';
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
    AlreadyRegisteredLink,
    DecisionDetails,
    FaithDetailsRegistered,
    FindOutMoreLink,
    MakeDecision,
    MessageText,
    MessageDialog,
    NextSteps,
    OtherThingsToDo,
    StillYourDecision,
    YourDecision,
  },
  data() {
    return {
      decision: this.$store.state.organDonation.originalRegistration.decision,
      decisionDetails: this.$store.state.organDonation.originalRegistration.decisionDetails,
      faithDeclaration: this.$store.state.organDonation.originalRegistration.faithDeclaration,
      isSomeOrgans: this.$store.getters['organDonation/isSomeOrgans'],
      state: this.$store.state.organDonation.originalRegistration.state,
    };
  },
  computed: {
    choices() {
      return get('choices')(this.decisionDetails);
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
    isConflicted() {
      return this.state === STATE_CONFLICTED && this.decision === DECISION_UNKNOWN;
    },
  },
  async asyncData({ store }) {
    await store.dispatch('organDonation/getReferenceData');
    await store.dispatch('organDonation/getRegistration');
  },
  created() {
    this.$store.dispatch('organDonation/amendCancel');
    this.$store.dispatch('organDonation/setAdditionalDetails', { ethnicityId: '', religionId: '' });
    this.$store.dispatch('organDonation/resetAcceptanceChecks');
    this.$store.dispatch('organDonation/reaffirmCancel');
    this.$store.dispatch('organDonation/withdrawCancel');
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/listmenu";
@import "../../style/spacings";

.info {
  &.appointedRep {
    p {
      padding-bottom: 0;
    }
  }
}
</style>
