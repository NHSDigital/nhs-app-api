<template>
  <div>
    <h2>{{ $t('organDonation.reviewYourDecision.confirmation.subheader') }}</h2>
    <error-group :show-error="showAccuracyError">
      <error-message v-if="showAccuracyError" id="accuracy-checkbox-error">
        {{ $t('organDonation.reviewYourDecision.confirmation.errors.accuracy') }}
      </error-message>
      <generic-checkbox v-model="isAccuracyAccepted"
                        value="accuracy-checkbox"
                        name="name"
                        :required="true"
                        checkbox-id="accuracy-checkbox"
                        @input="selectedValueChanged('accuracy-checkbox')">
        {{ $t('organDonation.reviewYourDecision.confirmation.accuracyText') }}
      </generic-checkbox>
    </error-group>
    <error-group :show-error="showPrivacyError">
      <error-message v-if="showPrivacyError" id="privacy-checkbox-error">
        {{ $t('organDonation.reviewYourDecision.confirmation.errors.privacy') }}
      </error-message>
      <generic-checkbox v-model="isPrivacyAccepted"
                        value="privacy-checkbox"
                        name="name"
                        :required="true"
                        checkbox-id="privacy-checkbox"
                        @input="selectedValueChanged('privacy-checkbox')">
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText1') }}
        <a :href="privacyUrl" target="_blank" rel="noopener noreferrer">
          {{ $t('organDonation.reviewYourDecision.confirmation.privacyLinkText') }}</a>
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText2') }}
      </generic-checkbox>
    </error-group>
  </div>
</template>

<script>
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import {
  ORGAN_DONATION_PRIVACY_URL,
} from '@/router/externalLinks';

export default {
  name: 'Confirmation',
  components: {
    ErrorGroup,
    ErrorMessage,
    GenericCheckbox,
  },
  props: {
    submitAttempted: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      accuracyCheckboxId: 'accuracy-checkbox',
      privacyCheckboxId: 'privacy-checkbox',
      privacyUrl: ORGAN_DONATION_PRIVACY_URL,
      checkboxes: [
        { code: 'accuracy-checkbox', id: 'accuracy-checkbox', selected: false },
        { code: 'privacy-checkbox', id: 'privacy-checkbox', selected: false },
      ],
      checkboxChoices: undefined,
    };
  },
  computed: {
    isAccuracyAccepted: {
      get() {
        return this.$store.state.organDonation.isAccuracyAccepted;
      },
      set(value) {
        this.$store.dispatch('organDonation/setAccuracyAcceptance', value);
      },
    },
    isPrivacyAccepted: {
      get() {
        return this.$store.state.organDonation.isPrivacyAccepted;
      },
      set(value) {
        this.$store.dispatch('organDonation/setPrivacyAcceptance', value);
      },
    },
    showAccuracyError() {
      return this.submitAttempted && !this.isAccuracyAccepted;
    },
    showPrivacyError() {
      return this.submitAttempted && !this.isPrivacyAccepted;
    },
    showInlineError() {
      return this.showError && !this.selected;
    },
  },
  methods: {
    selectedValueChanged(checkboxId) {
      const selectedCheckbox = this.checkboxes.find(x => x.id === checkboxId);
      this.checkboxes[this.checkboxes.indexOf(selectedCheckbox)].selected =
        !selectedCheckbox.selected;
      if (checkboxId === 'accuracy-checkbox') {
        this.$store.dispatch('organDonation/setAccuracyAcceptance', selectedCheckbox.selected);
      } else {
        this.$store.dispatch('organDonation/setPrivacyAcceptance', selectedCheckbox.selected);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  a{
    display: inline;
    font-weight: normal;
    vertical-align: unset;
  }
</style>
