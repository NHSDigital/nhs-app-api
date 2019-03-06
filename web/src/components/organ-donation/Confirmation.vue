<template>
  <div :class="$style.info">
    <h2>{{ $t('organDonation.reviewYourDecision.confirmation.subheader') }}</h2>
    <generic-checkbox :selected="isAccuracyAccepted" :class="$style.checkbox"
                      checkbox-id="accuracy-checkbox"
                      name="accuracy" @click="toggleAccuracy">
      <span :class="$style['checkbox-label']">
        {{ $t('organDonation.reviewYourDecision.confirmation.accuracyText') }}
        <span v-if="isAccuracyStarVisible" :class="$style.red">*</span>
      </span>
    </generic-checkbox>

    <generic-checkbox :selected="isPrivacyAccepted" checkbox-id="privacy-checkbox"
                      name="privacy" @click="togglePrivacy">
      <span :class="$style['checkbox-label']">
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText1') }}
        <a :href="privacyUrl" target="_blank">
          {{ $t('organDonation.reviewYourDecision.confirmation.privacyLinkText') }}
        </a>
        {{ $t('organDonation.reviewYourDecision.confirmation.privacyText2') }}
        <span v-if="isPrivacyStarVisible" :class="$style.red">*</span>
      </span>
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
    submitAttempted: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      privacyUrl: 'https://www.nhsbt.nhs.uk/privacy/',
    };
  },
  computed: {
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
