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
      <decision-details v-if="isOptInDecision && !allOrgans"
                        :choices="currentChoices"/>
      <amend-decision-link :class="$style.amendDecision"/>
      <next-steps :is-opt-in-decision="isOptInDecision"/>
    </div>
    <other-things-to-do :can-withdraw="!isConflicted"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NextSteps from '@/components/organ-donation/NextSteps';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN, STATE_CONFLICTED } from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    AmendDecisionLink,
    DecisionDetails,
    MessageText,
    MessageDialog,
    NextSteps,
    OtherThingsToDo,
    YourDecision,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
      allOrgans: !!get('organDonation.registration.decisionDetails.all')(this.$store.state),
      currentChoices: get('organDonation.registration.decisionDetails.choices')(this.$store.state) || {},
      decision: get('organDonation.registration.decision')(this.$store.state),
      identifier: get('organDonation.registration.identifier')(this.$store.state),
      state: get('organDonation.registration.state')(this.$store.state),
    };
  },
  computed: {
    isOptInDecision() {
      return this.decision === DECISION_OPT_IN;
    },
    isConflicted() {
      return this.state === STATE_CONFLICTED && !!this.identifier;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/spacings";

.amendDecision {
  margin-bottom: $three;
}

.messageText {
  padding-bottom: 1em !important;
}
</style>
