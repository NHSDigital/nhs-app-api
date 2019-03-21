<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="showErrors" id="errors">
      <message-text data-purpose="error-heading">
        {{ $t('organDonation.reviewYourDecision.errorMsgHeader') }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li v-for="error in validationErrors" :key="error">{{ $t(error) }}</li>
      </message-list>
    </message-dialog>

    <h2>{{ $t('organDonation.reviewYourDecision.header') }}</h2>
    <personal-details
      :name="$store.state.organDonation.registration.nameFull"
      :date-of-birth="$store.state.organDonation.registration.dateOfBirth"
      :gender="$store.state.organDonation.registration.gender"
      :nhs-number="$store.state.organDonation.registration.nhsNumber"
      :address="$store.state.organDonation.registration.addressFull"/>
    <hr :class="$style.rule" aria-hidden="true">
    <contact-details :address="$store.state.organDonation.registration.addressFull"/>
    <hr :class="$style.rule" aria-hidden="true">
    <div v-if="showAdditionalDetails">
      <additional-information
        :ethnicity-id="$store.state.organDonation.additionalDetails.ethnicityId"
        :religion-id="$store.state.organDonation.additionalDetails.religionId"
        :reference-data="$store.state.organDonation.referenceData"/>
      <hr :class="$style.rule" aria-hidden="true">
    </div>
    <your-decision :decision-details="$store.state.organDonation.registration.decisionDetails"
                   :decision="$store.state.organDonation.registration.decision"
                   :is-withdrawing="isWithdrawing"/>
    <div v-if="isWithdrawing" :class="$style['mb-3']">
      <h3>{{ $t('organDonation.reviewYourDecision.withdraw.subheader') }}</h3>
      <p>
        {{ $t('organDonation.reviewYourDecision.withdraw.body') }}
      </p>
      <hr :class="$style.rule" aria-hidden="true">
    </div>
    <div v-else-if="isOptInDecision">
      <decision-details v-if="isSomeOrgans"
                        :choices="currentChoices"/>
      <hr :class="$style.rule" aria-hidden="true">
      <div id="faithDetails">
        <faith-details :declaration="$store.state.organDonation.registration.faithDeclaration"/>
        <hr :class="$style.rule" aria-hidden="true">
      </div>
    </div>
    <hr v-else :class="$style.rule" aria-hidden="true">
    <confirmation :submit-attempted="submitAttempted"/>
    <generic-button id="submit-button"
                    :class="[$style.button, $style.green]"
                    @click="clickSubmit">
      {{ $t('organDonation.reviewYourDecision.submitButton') }}
    </generic-button>
    <back-button />
  </div>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import get from 'lodash/fp/get';
import PersonalDetails from '@/components/organ-donation/PersonalDetails';
import ContactDetails from '@/components/organ-donation/ContactDetails';
import AdditionalInformation from '@/components/organ-donation/AdditionalInformation';
import BackButton from '@/components/BackButton';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import Confirmation from '@/components/organ-donation/Confirmation';
import FaithDetails from '@/components/organ-donation/FaithDetails';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import { EnsureCanSubmit } from '@/components/organ-donation/EnsureDecisionMixin';

export default {
  components: {
    AdditionalInformation,
    BackButton,
    Confirmation,
    ContactDetails,
    DecisionDetails,
    FaithDetails,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    PersonalDetails,
    YourDecision,
  },
  mixins: [EnsureCanSubmit],
  data() {
    return {
      currentChoices: get('$store.state.organDonation.registration.decisionDetails.choices')(this) || {},
      isOptInDecision: this.$store.state.organDonation.registration.decision === DECISION_OPT_IN,
      isSomeOrgans: this.$store.getters['organDonation/isSomeOrgans'],
      isWithdrawing: this.$store.state.organDonation.isWithdrawing,
      submitAttempted: false,
    };
  },
  computed: {
    isAccuracyAccepted() {
      return this.$store.state.organDonation.isAccuracyAccepted;
    },
    isPrivacyAccepted() {
      return this.$store.state.organDonation.isPrivacyAccepted;
    },
    showAdditionalDetails() {
      return !this.isWithdrawing && (!this.$store.state.organDonation.isReaffirming ||
        this.wasSomeOrgans);
    },
    showErrors() {
      return this.submitAttempted && !isEmpty(this.validationErrors);
    },
    validationErrors() {
      const errors = [];
      if (!this.isAccuracyAccepted) {
        errors.push('organDonation.reviewYourDecision.confirmation.errors.accuracy');
      }

      if (!this.isPrivacyAccepted) {
        errors.push('organDonation.reviewYourDecision.confirmation.errors.privacy');
      }
      return errors;
    },
    wasSomeOrgans() {
      return this.$store.state.organDonation.originalRegistration.decision === DECISION_OPT_IN &&
        get('$store.state.organDonation.originalRegistration.decisionDetails.all')(this) === false;
    },
  },
  created() {
    this.$store.dispatch('organDonation/resetAcceptanceChecks');
  },
  methods: {
    clickSubmit() {
      this.submitAttempted = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }

      this.$store.dispatch('organDonation/submitDecision');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";
  @import "../../style/accessibility";
  @import "../../style/textstyles";

  .rule {
    border-top: solid 1px $mid_grey;
    margin-top: 1em;
    margin-bottom: 0.5em;
  }
</style>
