<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="isConflicted">
      <message-dialog :icon-text="$t('organDonation.viewDecision.decisionSubmitted.dialogText')"
                      message-id="success-dialog" message-type="success">
        <message-text>
          {{ $t('organDonation.viewDecision.decisionSubmitted.messageText') }}</message-text>
      </message-dialog>
      <strong>{{ $t('organDonation.viewDecision.decisionSubmitted.registrationHeader') }}</strong>
      <p :class="$style.messageText">
        {{ $t('organDonation.viewDecision.decisionSubmitted.registrationText') }}</p>
    </div>
    <div v-else>
      <message-dialog message-id="success-dialog" message-type="success">
        <message-text>{{ $t('organDonation.viewDecision.successMessageText') }}</message-text>
      </message-dialog>
      <your-decision :decision-details="$store.state.organDonation.registration.decisionDetails"
                     :decision="$store.state.organDonation.registration.decision"/>
      <decision-details v-if="isOptInDecision && isSomeOrgans"
                        :choices="currentChoices"/>
      <faith-details-registered v-if="isOptInDecision"
                                :declaration="faithDeclaration"/>
      <still-your-decision :show-amend="true" :show-reaffirm="false" />
      <next-steps :is-opt-in-decision="isOptInDecision"/>
    </div>
    <other-things-to-do :can-withdraw="!isConflicted"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NextSteps from '@/components/organ-donation/NextSteps';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import StillYourDecision from '@/components/organ-donation/StillYourDecision';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN, STATE_CONFLICTED } from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    DecisionDetails,
    FaithDetailsRegistered,
    MessageText,
    MessageDialog,
    NextSteps,
    OtherThingsToDo,
    StillYourDecision,
    YourDecision,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
      currentChoices: get('organDonation.registration.decisionDetails.choices')(this.$store.state) || {},
      faithDeclaration: this.$store.state.organDonation.registration.faithDeclaration,
      isOptInDecision: get('organDonation.registration.decision')(this.$store.state) === DECISION_OPT_IN,
      isSomeOrgans: this.$store.getters['organDonation/isSomeOrgans'],
    };
  },
  computed: {
    isConflicted() {
      return this.$store.state.organDonation.registration.state === STATE_CONFLICTED
        && !!this.$store.state.organDonation.registration.identifier;
    },
  },
  created() {
    this.$store.dispatch('organDonation/amendCancel');
    this.$store.dispatch('organDonation/reaffirmCancel');
    this.$store.dispatch('organDonation/setAdditionalDetails', { ethnicityId: '', religionId: '' });
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";

.messageText {
  padding-bottom: 1em !important;
}
</style>
