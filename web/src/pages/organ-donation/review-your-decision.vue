<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="showErrors" id="errors">
      <message-text data-purpose="error-heading">
        {{ $t('organDonation.reviewYourDecision.errorMsgHeader') }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li v-for="error in validationErrors" :key="error">{{ $t(error) }}</li>
      </message-list>
    </message-dialog>

    <h2>{{ $t('organDonation.reviewYourDecision.header') }}</h2>
    <hr :class="$style.rule" aria-hidden="true">
    <about-you :name="$store.state.organDonation.registration.nameFull"
               :date-of-birth="$store.state.organDonation.registration.dateOfBirth"
               :gender="$store.state.organDonation.registration.gender"
               :nhs-number="$store.state.organDonation.registration.nhsNumber"
               :address="$store.state.organDonation.registration.addressFull"/>
    <hr :class="$style.rule" aria-hidden="true">
    <additional-information :ethnicity-id="$store.state.organDonation.additionalDetails.ethnicityId"
                            :religion-id="$store.state.organDonation.additionalDetails.religionId"
                            :reference-data="$store.state.organDonation.referenceData"/>
    <hr :class="$style.rule" aria-hidden="true">
    <your-decision :decision-details="$store.state.organDonation.registration.decisionDetails"
                   :decision="$store.state.organDonation.registration.decision"/>
    <decision-details v-if="isOptInDecision && !allOrgans"
                      :choices="currentChoices"/>
    <hr :class="$style.rule" aria-hidden="true">
    <div v-if="isOptInDecision" id="faithDetails">
      <faith-details :declaration="$store.state.organDonation.registration.faithDeclaration"/>
      <hr :class="$style.rule" aria-hidden="true">
    </div>
    <confirmation :is-accuracy-star-visible="isAccuracyStarVisible"
                  :is-privacy-star-visible="isPrivacyStarVisible" />
    <generic-button id="submit-button"
                    :class="[$style.button, $style.green]"
                    @click="clickSubmit">
      {{ submitButtonText }}
    </generic-button>
    <back-button />
  </div>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import get from 'lodash/fp/get';
import AboutYou from '@/components/organ-donation/AboutYou';
import AdditionalInformation from '@/components/organ-donation/AdditionalInformation';
import BackButton from '@/components/BackButton';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import Confirmation from '@/components/organ-donation/Confirmation';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import FaithDetails from '@/components/organ-donation/FaithDetails';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import YourDecision from '@/components/organ-donation/YourDecision';
import { ORGAN_DONATION_VIEW_DECISION } from '@/lib/routes';
import { DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    AboutYou,
    AdditionalInformation,
    BackButton,
    Confirmation,
    DecisionDetails,
    FaithDetails,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    YourDecision,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
      submitAttempted: false,
    };
  },
  computed: {
    allOrgans() {
      return !!get('all')(this.$store.state.organDonation.registration.decisionDetails);
    },
    currentChoices() {
      return get('choices')(this.$store.state.organDonation.registration.decisionDetails) || {};
    },
    isAccuracyAccepted() {
      return this.$store.state.organDonation.isAccuracyAccepted;
    },
    isAccuracyStarVisible() {
      return this.submitAttempted && !this.isAccuracyAccepted;
    },
    isOptInDecision() {
      return this.$store.state.organDonation.registration.decision === DECISION_OPT_IN;
    },
    isPrivacyAccepted() {
      return this.$store.state.organDonation.isPrivacyAccepted;
    },
    isPrivacyStarVisible() {
      return this.submitAttempted && !this.isPrivacyAccepted;
    },
    showErrors() {
      return this.submitAttempted && !isEmpty(this.validationErrors);
    },
    submitButtonText() {
      return this.isOptInDecision
        ? this.$t('organDonation.reviewYourDecision.submitButton')
        : this.$t('organDonation.reviewYourDecision.submitNoButton');
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
  },
  created() {
    this.$store.dispatch('organDonation/resetAcceptanceChecks');
  },
  methods: {
    async clickSubmit() {
      this.submitAttempted = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }

      const action = this.$store.state.organDonation.isAmending ? 'put' : 'post';
      await this.$store.dispatch(`organDonation/${action}Registration`);
      this.$router.push(ORGAN_DONATION_VIEW_DECISION.path);
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
