<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="showErrors" id="errors">
      <message-text v-for="error in validationErrors" :key="error">
        {{ $t(error) }}
      </message-text>
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
    <your-decision :decision="$store.state.organDonation.registration.decision"/>
    <hr :class="$style.rule" aria-hidden="true">
    <confirmation :is-accuracy-star-visible="isAccuracyStarVisible"
                  :is-privacy-star-visible="isPrivacyStarVisible" />
    <generic-button id="submit-button"
                    :class="$style.green"
                    @click="clickSubmit">
      {{ $t('organDonation.reviewYourDecision.submitButton') }}
    </generic-button>
    <generic-button id="back-button"
                    :class="[$style.button, $style.grey]"
                    @click.prevent="clickBack">
      {{ $t('organDonation.reviewYourDecision.backButton') }}
    </generic-button>
  </div>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import GenericButton from '@/components/widgets/GenericButton';
import AboutYou from '@/components/organ-donation/AboutYou';
import AdditionalInformation from '@/components/organ-donation/AdditionalInformation';
import Confirmation from '@/components/organ-donation/Confirmation';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import YourDecision from '@/components/organ-donation/YourDecision';
import { ORGAN_DONATION_ADDITIONAL_DETAILS, ORGAN_DONATION_YOUR_CHOICE } from '@/lib/routes';
import { DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';

export default {
  components: {
    AboutYou,
    AdditionalInformation,
    Confirmation,
    GenericButton,
    MessageDialog,
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
    backRoute() {
      return this.$store.state.organDonation.registration.decision === DECISION_OPT_OUT
        ? ORGAN_DONATION_ADDITIONAL_DETAILS.path
        : ORGAN_DONATION_YOUR_CHOICE.path;
    },
    isAccuracyAccepted() {
      return this.$store.state.organDonation.isAccuracyAccepted;
    },
    isAccuracyStarVisible() {
      return this.submitAttempted && !this.isAccuracyAccepted;
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
  methods: {
    clickBack() {
      this.$router.push(this.backRoute);
    },
    clickSubmit() {
      this.submitAttempted = true;
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
