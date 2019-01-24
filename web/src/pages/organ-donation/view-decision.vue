<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog message-id="success-dialog" message-type="success">
      <message-text>We have updated your decision</message-text>
    </message-dialog>
    <your-decision :decision-details="$store.state.organDonation.registration.decisionDetails"
                   :decision="$store.state.organDonation.registration.decision"/>
    <decision-details v-if="isOptInDecision && !allOrgans"
                      :choices="currentChoices"/>
    <other-things-to-do/>
  </div>
</template>

<script>
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import get from 'lodash/fp/get';

export default {
  components: {
    DecisionDetails,
    MessageText,
    MessageDialog,
    OtherThingsToDo,
    YourDecision,
  },
  mixins: [EnsureDecisionMixin],

  computed: {
    allOrgans() {
      return !!get('all')(this.$store.state.organDonation.registration.decisionDetails);
    },
    currentChoices() {
      return get('choices')(this.$store.state.organDonation.registration.decisionDetails) || {};
    },
    isOptInDecision() {
      return this.$store.state.organDonation.registration.decision === DECISION_OPT_IN;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";

</style>
