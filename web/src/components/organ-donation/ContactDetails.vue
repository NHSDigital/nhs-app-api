<template>
  <div :class="$style.info">
    <h3>{{ $t('organDonation.reviewYourDecision.contactDetails.subheader') }}</h3>
    <p :class="$style['pt-3']">{{ $t('organDonation.reviewYourDecision.contactDetails.text') }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.contactDetails.addressheader') }}</h4>
    <p>{{ address }}</p>
    <p :class="$style['pt-3']">
      {{ $t('organDonation.reviewYourDecision.contactDetails.changeDetailsText') }}
    </p>
  </div>
</template>

<script>
export default {
  name: 'ContactDetails',
  props: {
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
