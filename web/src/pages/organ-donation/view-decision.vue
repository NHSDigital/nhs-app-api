<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="showDecisionSubmitted">
      <message-dialog :icon-text="$t('organDonation.viewDecision.decisionSubmitted.dialogText')"
                      message-id="success-dialog" message-type="success">
        <message-text>
          {{ $t('organDonation.viewDecision.decisionSubmitted.messageText') }}</message-text>
      </message-dialog>
      <strong>{{ $t('organDonation.viewDecision.decisionSubmitted.registrationHeader') }}</strong>
      <p :class="$style.messageText">
        {{ $t('organDonation.viewDecision.decisionSubmitted.registrationText') }}</p>
    </div>
    <div v-if="showYourDecision">
      <message-dialog message-id="success-dialog" message-type="success">
        <message-text>{{ $t('organDonation.viewDecision.successMessageText') }}</message-text>
      </message-dialog>
      <your-decision :decision-details="$store.state.organDonation.registration.decisionDetails"
                     :decision="$store.state.organDonation.registration.decision"/>
      <decision-details v-if="isOptInDecision && !allOrgans"
                        :choices="currentChoices"/>
      <amend-decision-link :class="$style.amendDecision"/>
    </div>
    <other-things-to-do/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN, STATE_CONFLICTED, STATE_OK } from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    AmendDecisionLink,
    DecisionDetails,
    MessageText,
    MessageDialog,
    OtherThingsToDo,
    YourDecision,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
      decision: get('organDonation.registration.decision')(this.$store.state),
      identifier: get('organDonation.registration.identifier')(this.$store.state),
      state: get('organDonation.registration.state')(this.$store.state),
    };
  },
  computed: {
    allOrgans() {
      return !!get('all')(this.$store.state.organDonation.registration.decisionDetails);
    },
    currentChoices() {
      return get('choices')(this.$store.state.organDonation.registration.decisionDetails) || {};
    },
    isOptInDecision() {
      return this.decision === DECISION_OPT_IN;
    },
    showDecisionSubmitted() {
      return this.state === STATE_CONFLICTED && this.identifier;
    },
    showYourDecision() {
      return this.state === STATE_OK;
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
