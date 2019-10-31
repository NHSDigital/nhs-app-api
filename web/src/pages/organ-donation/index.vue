<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div v-if="isConflicted || hasExistingDecision">
      <div v-if="isConflicted" :class="$style['mb-3']">
        <message-dialog :icon-text="$t('organDonation.viewDecision.conflictedState.dialogText')"
                        message-id="success-dialog" message-type="success">
          <message-text>
            {{ $t('organDonation.viewDecision.conflictedState.messageText') }}</message-text>
        </message-dialog>
        <h2>{{ $t('organDonation.viewDecision.conflictedState.registrationHeader') }}</h2>
        <p>
          {{ $t('organDonation.viewDecision.conflictedState.registrationText') }}</p>
      </div>
      <div v-else :class="$style['mb-3']">
        <decision-info :decision="decision"
                       :decision-details="decisionDetails"
                       header-key="organDonation.registered.yourDecision.subheader"/>

        <faith-details-registered v-if="hasExistingOptIn" :declaration="faithDeclaration"/>

        <still-your-decision :is-some-organs="isSomeOrgans"
                             :show-amend="true"
                             :show-reaffirm="!hasAppointedRep"/>

        <div v-if="hasAppointedRep" :class="[$style.info, $style.appointedRep, $style['mt-3']]">
          <p>{{ $t('organDonation.registered.appointedRep.phoneLabel') }}</p>
          <span role="text" aria-label="zero three zero zero one two three two three two three">
            0300 123 2323
          </span>
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
import DecisionInfo from '@/components/organ-donation/DecisionInfo';
import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NextSteps from '@/components/organ-donation/NextSteps';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import StillYourDecision from '@/components/organ-donation/StillYourDecision';
import {
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
  STATE_CONFLICTED,
} from '@/store/modules/organDonation/mutation-types';
import { INDEX } from '@/lib/routes';
import { isNativeApp } from '@/components/NativeOnlyMixin';

const load = async (store) => {
  await store.dispatch('organDonation/getReferenceData');
  await store.dispatch('organDonation/getRegistration');
};

export default {
  components: {
    AlreadyRegisteredLink,
    DecisionInfo,
    FaithDetailsRegistered,
    FindOutMoreLink,
    MakeDecision,
    MessageText,
    MessageDialog,
    NextSteps,
    OtherThingsToDo,
    StillYourDecision,
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
    faithDeclaration() {
      return this.$store.state.organDonation.originalRegistration.faithDeclaration;
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
    isSomeOrgans() {
      return this.$store.getters['organDonation/isSomeOrgans'];
    },
    state() {
      return this.$store.state.organDonation.originalRegistration.state;
    },
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      load(this.$store);
    },
  },
  async asyncData({ redirect, route, store }) {
    if (!isNativeApp({ route, store })) {
      redirect(INDEX.path);
    } else {
      await load(store);
    }
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
