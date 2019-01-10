<template>
  <div :class="$style.info">
    <h2>{{ $t('organDonation.reviewYourDecision.confirmation.subheader') }}</h2>
    <generic-checkbox :selected="isAccuracyAccepted"
                      v-model="isAccuracyAccepted"
                      checkbox-id="accuracy-checkbox"
                      name="accuracy"
                      @click="checkAccuracy">
      <label :class="$style['checkbox-label']" for="accuracy-accuracy-checkbox">
        {{ $t('organDonation.reviewYourDecision.confirmation.accuracyText') }}
      </label>
    </generic-checkbox>

    <generic-checkbox :selected="isPrivacyAccepted"
                      v-model="isPrivacyAccepted"
                      checkbox-id="privacy-checkbox"
                      name="privacy"
                      @click="checkPrivacy">
      <label :class="$style['checkbox-label']" for="privacy-privacy-checkbox">
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText1') }}
        <a :href="privacyUrl" target="_blank">
          {{ $t('organDonation.reviewYourDecision.confirmation.privacyLinkText') }}
        </a>
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText2') }}
      </label>
    </generic-checkbox>
  </div>
</template>

<script>
import GenericCheckbox from '@/components/widgets/GenericCheckbox';

export default {
  components: {
    GenericCheckbox,
  },
  data() {
    return {
      isAccuracyAccepted: false,
      isPrivacyAccepted: false,
      privacyUrl: 'https://www.nhsbt.nhs.uk/privacy/',
    };
  },
  methods: {
    checkAccuracy() {
      this.isAccuracyAccepted = !this.isAccuracyAccepted;
    },

    checkPrivacy() {
      this.isPrivacyAccepted = !this.isPrivacyAccepted;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";
  @import "../../style/forms";
  @import "../../style/accessibility";

  .checkbox-label {
    font-weight: normal;

    a {
      display: inline-block;
    }
  }
</style>
