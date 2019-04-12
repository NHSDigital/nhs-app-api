<template>
  <div :class="$style.info">
    <h2>{{ $t('organDonation.reviewYourDecision.confirmation.subheader') }}</h2>
    <checkbox v-model="isAccuracyAccepted"
              :checkbox-id="accuracyCheckboxId"
              :show-error="showAccuracyError"
              :error-message="$t('organDonation.reviewYourDecision.confirmation.errors.accuracy')">
      <span :class="$style['checkbox-label']">
        {{ $t('organDonation.reviewYourDecision.confirmation.accuracyText') }}
      </span>
    </checkbox>
    <checkbox v-model="isPrivacyAccepted"
              :checkbox-id="privacyCheckboxId"
              :show-error="showPrivacyError"
              :error-message="$t('organDonation.reviewYourDecision.confirmation.errors.privacy')">
      <span :class="$style['checkbox-label']">
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText1') }}
        <a :href="privacyUrl" target="_blank">
          {{ $t('organDonation.reviewYourDecision.confirmation.privacyLinkText') }}
        </a>
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText2') }}
      </span>
    </checkbox>
  </div>
</template>

<script>
import Checkbox from '@/components/Checkbox';

export default {
  components: {
    Checkbox,
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
      privacyUrl: this.$store.app.$env.ORGAN_DONATION_PRIVACY_URL,
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
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";
  @import "../../style/forms";

  .checkbox-label {
    font-weight: normal;

    a {
      display: inline-block;
      padding-top: 0;
      padding-bottom: 0;
      vertical-align: baseline;
    }
  }
</style>
