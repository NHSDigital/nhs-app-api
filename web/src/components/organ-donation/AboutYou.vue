<template>
  <div>
    <h3>{{ $t('organDonation.reviewYourDecision.aboutYou.subheader') }}</h3>
    <h4>{{ $t('organDonation.reviewYourDecision.aboutYou.nameheader') }}</h4>
    <p>{{ name }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.aboutYou.dateofbirthheader') }}</h4>
    <p>{{ dateOfBirth | longDate }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.aboutYou.genderheader') }}</h4>
    <p>{{ gender }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.aboutYou.nhsnumberheader') }}</h4>
    <p>{{ nhsNumber }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.aboutYou.addressheader') }}</h4>
    <p>{{ address }}</p>
    <p :class="$style.desc">{{ $t('organDonation.reviewYourDecision.aboutYou.description') }}</p>
  </div>
</template>

<script>
export default {
  name: 'AboutYou',
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
    address: {
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


  h3 {
    margin: 0;
    padding: 0;
  }

  h4 {
    margin: 0;
    padding: $three 0 $one 0;
  }

  .desc {
    padding-top: $three;
  }
</style>
