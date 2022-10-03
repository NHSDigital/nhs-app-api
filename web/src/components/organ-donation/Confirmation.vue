<template>
  <div>
    <h2>{{ $t('organDonation.confirmation.confirmation') }}</h2>
    <error-group :show-error="showAccuracyError">
      <error-message v-if="showAccuracyError" id="accuracy-checkbox-error">
        {{ $t('organDonation.confirmation.checkYourInformation') }}
      </error-message>
      <generic-checkbox value="accuracy-checkbox"
                        name="name"
                        :required="true"
                        :a-described-by="showAccuracyError ?
                          'accuracy-checkbox-error' : undefined"
                        checkbox-id="accuracy-checkbox"
                        @input="selectedValueChanged('accuracy-checkbox')">
        {{ $t('organDonation.confirmation.iConfirmTheInformation') }}
      </generic-checkbox>
    </error-group>
    <error-group :show-error="showPrivacyError">
      <error-message v-if="showPrivacyError" id="privacy-checkbox-error">
        {{ $t('organDonation.confirmation.readThePrivacyPolicyAndConfirmConstent') }}
      </error-message>
      <generic-checkbox value="privacy-checkbox"
                        name="name"
                        :required="true"
                        :a-described-by="showPrivacyError ? 'privacy-checkbox-error' : undefined"
                        checkbox-id="privacy-checkbox"
                        @input="selectedValueChanged('privacy-checkbox')">
        {{ $t('organDonation.confirmation.iHaveReadThe') }}
        <a :href="privacyUrl" target="_blank" rel="noopener noreferrer">
          {{ $t('organDonation.confirmation.privacyPolicy') }}</a>
        {{ $t('organDonation.confirmation.andGiveConsent') }}
      </generic-checkbox>
    </error-group>
  </div>
</template>

<script>
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';

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
    isAccuracyAccepted: {
      type: Boolean,
      required: true,
    },
    isPrivacyAccepted: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      accuracyCheckboxId: 'accuracy-checkbox',
      privacyCheckboxId: 'privacy-checkbox',
      privacyUrl: this.$store.$env.ORGAN_DONATION_PRIVACY_URL,
      checkboxes: [
        { code: 'accuracy-checkbox', id: 'accuracy-checkbox', selected: false },
        { code: 'privacy-checkbox', id: 'privacy-checkbox', selected: false },
      ],
      checkboxChoices: undefined,
    };
  },
  computed: {
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
  @import "@/style/custom/confirmation";
</style>
