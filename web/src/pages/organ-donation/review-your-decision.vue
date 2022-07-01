<template>
  <div v-if="showTemplate" id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <form-error-summary v-if="showErrors"
                          :header-locale-ref="'organDonation.thereIsAProblem'"
                          :errors="validationErrors"/>

      <h2>{{ $t('organDonation.reviewYourDecision.aboutYou') }}</h2>
      <personal-details
        :name="$store.state.organDonation.registration.nameFull"
        :date-of-birth="$store.state.organDonation.registration.dateOfBirth"
        :gender="$store.state.organDonation.registration.gender"
        :nhs-number="$store.state.organDonation.registration.nhsNumber"
        :address="$store.state.organDonation.registration.addressFull"/>
      <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      <contact-details :address="$store.state.organDonation.registration.addressFull"/>
      <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      <div v-if="showAdditionalDetails">
        <additional-information
          :ethnicity-id="$store.state.organDonation.additionalDetails.ethnicityId"
          :religion-id="$store.state.organDonation.additionalDetails.religionId"
          :reference-data="$store.state.organDonation.referenceData"/>
        <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      </div>
      <decision-info :decision-details="$store.state.organDonation.registration.decisionDetails"
                     :decision="$store.state.organDonation.registration.decision"
                     :is-withdrawing="isWithdrawing"/>
      <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      <div v-if="!isWithdrawing && isOptInDecision" id="faithDetails">
        <faith-details :declaration="$store.state.organDonation.registration.faithDeclaration"/>
        <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      </div>
      <confirmation :submit-attempted="submitAttempted"/>
      <generic-button id="submit-button"
                      class="nhsuk-button"
                      :disabled="isDisabled"
                      click-delay="medium"
                      @click="clickSubmit">
        {{ $t('organDonation.reviewYourDecision.submitMyDecision') }}
      </generic-button>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="backLink"
                                 :button-text="'generic.back'"
                                 @clickAndPrevent="backClicked"/>
    </div>
  </div>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import get from 'lodash/fp/get';
import AdditionalInformation from '@/components/organ-donation/AdditionalInformation';
import ContactDetails from '@/components/organ-donation/ContactDetails';
import DecisionInfo from '@/components/organ-donation/DecisionInfo';
import DynamicBackLinkMixin from '@/components/organ-donation/DynamicBackLinkMixin';
import Confirmation from '@/components/organ-donation/Confirmation';
import FaithDetails from '@/components/organ-donation/FaithDetails';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import FormErrorSummary from '@/components/FormErrorSummary';
import PersonalDetails from '@/components/organ-donation/PersonalDetails';
import { DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import { EnsureCanSubmit } from '@/components/organ-donation/EnsureDecisionMixin';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

export default {
  components: {
    AdditionalInformation,
    Confirmation,
    ContactDetails,
    DecisionInfo,
    FaithDetails,
    GenericButton,
    DesktopGenericBackLink,
    FormErrorSummary,
    PersonalDetails,
  },
  mixins: [EnsureCanSubmit, DynamicBackLinkMixin],
  data() {
    return {
      isDisabled: false,
      isOptInDecision: this.$store.state.organDonation.registration.decision === DECISION_OPT_IN,
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
        errors.push(this.$t('organDonation.reviewYourDecision.checkInformationAndConfirm'));
      }

      if (!this.isPrivacyAccepted) {
        errors.push(this.$t('organDonation.reviewYourDecision.readPrivacyStatmentAndConsent'));
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
      this.submitAttempted = false;

      this.$nextTick(() => {
        this.submitAttempted = true;

        if (this.showErrors) {
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
          return;
        }

        this.isDisabled = true;

        this.$store.dispatch('organDonation/submitDecision');
      });
    },
  },
};
</script>
