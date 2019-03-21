<template>
  <div :class="$style.info">
    <h3>{{ $t('organDonation.reviewYourDecision.personalDetails.subheader') }}</h3>
    <h4>{{ $t('organDonation.reviewYourDecision.personalDetails.nameheader') }}</h4>
    <p>{{ name }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.personalDetails.dateofbirthheader') }}</h4>
    <p>{{ dateOfBirth | longDate }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.personalDetails.genderheader') }}</h4>
    <p>{{ gender }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.personalDetails.nhsnumberheader') }}</h4>
    <p>{{ nhsNumber }}</p>
    <p :class="$style.pt-3">
      {{ $t('organDonation.reviewYourDecision.personalDetails.description') }}
    </p>
  </div>
</template>

<script>
export default {
  name: 'PersonalDetails',
  props: {
    name: {
      type: String,
      default: '',
    },
    dateOfBirth: {
      type: String,
      default: '',
    },
    gender: {
      type: String,
      default: '',
    },
    nhsNumber: {
      type: String,
      default: '',
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";
  @import '../../style/fonts';
  @import "../../style/spacings";

  .info {
    h3 {
      margin: 0;
      padding: 0;
    }

    h4 {
      margin: 0;
      padding: $three 0 0 0;
    }
  }
</style>
