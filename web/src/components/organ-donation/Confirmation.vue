<template>
  <div :class="$style.info">
    <h2>{{ $t('organDonation.reviewYourDecision.confirmation.subheader') }}</h2>
    <generic-checkbox :selected="$store.state.organDonation.isAccuracyAccepted"
                      v-model="$store.state.organDonation.isAccuracyAccepted"
                      checkbox-id="accuracy-checkbox"
                      name="accuracy"
                      @click="toggleAccuracy">
      <label :class="$style['checkbox-label']" for="accuracy-accuracy-checkbox">
        {{ $t('organDonation.reviewYourDecision.confirmation.accuracyText') }}
        <span v-if="isAccuracyStarVisible" :class="$style.red">*</span>
      </label>
    </generic-checkbox>

    <generic-checkbox :selected="$store.state.organDonation.isPrivacyAccepted"
                      v-model="$store.state.organDonation.isPrivacyAccepted"
                      checkbox-id="privacy-checkbox"
                      name="privacy"
                      @click="togglePrivacy">
      <label :class="$style['checkbox-label']" for="privacy-privacy-checkbox">
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText1') }}
        <a :href="privacyUrl" target="_blank">
          {{ $t('organDonation.reviewYourDecision.confirmation.privacyLinkText') }}
        </a>
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText2') }}
        <span v-if="isPrivacyStarVisible" :class="$style.red">*</span>
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
  props: {
    isAccuracyStarVisible: {
      type: Boolean,
      default: false,
    },
    isPrivacyStarVisible: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      privacyUrl: 'https://www.nhsbt.nhs.uk/privacy/',
    };
  },
  methods: {
    toggleAccuracy() {
      this.$store.dispatch('organDonation/toggleAccuracyAcceptance');
    },

    togglePrivacy() {
      this.$store.dispatch('organDonation/togglePrivacyAcceptance');
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
      padding-top: 0;
      padding-bottom: 0;
      vertical-align: baseline;
    }
  }
  .red {
    color: $red;
    font-weight: bold;
  }
</style>
