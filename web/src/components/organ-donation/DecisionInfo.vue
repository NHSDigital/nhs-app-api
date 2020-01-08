<template>
  <div>
    <your-decision :decision="decision"
                   :decision-details="decisionDetails"
                   :is-withdrawing="isWithdrawing"
                   :header-key="headerKey"/>

    <withdrawing v-if="isWithdrawing" />

    <decision-details v-else-if="isOptInDecision"
                      :choices="currentChoices"
                      :is-some-organs="isSomeOrgans"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import Withdrawing from '@/components/organ-donation/Withdrawing';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'DecisionInfo',
  components: {
    DecisionDetails,
    Withdrawing,
    YourDecision,
  },
  props: {
    decision: {
      type: String,
      required: true,
    },
    decisionDetails: {
      type: Object,
      default: () => {},
    },
    headerKey: {
      type: String,
      default: undefined,
    },
    isWithdrawing: {
      type: Boolean,
    },
  },
  data() {
    return {
      currentChoices: get('choices')(this.decisionDetails) || {},
      isOptInDecision: this.decision === DECISION_OPT_IN,
      isSomeOrgans: this.$store.getters['organDonation/isSomeOrgans'],
    };
  },
};
</script>
