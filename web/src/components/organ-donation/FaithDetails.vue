<template>
  <div :class="$style.info">
    <h2>{{ $t('organDonation.reviewYourDecision.faith.subheader') }}</h2>
    <p><b>{{ $t('organDonation.reviewYourDecision.faith.description') }}</b></p>
    <span>{{ $t(`organDonation.reviewYourDecision.faith.declaration.${declaration}`) }}</span>
  </div>
</template>
<script>
export default {
  props: {
    declaration: {
      type: String,
      required: true,
    },
  },
};
</script>
<style module lang="scss">
@import "../../style/info";
</style>
