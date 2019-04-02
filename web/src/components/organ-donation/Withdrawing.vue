<template>
  <div :class="$style['mb-3']">
    <h3>{{ $t('organDonation.reviewYourDecision.withdraw.subheader') }}</h3>
    <p>
      {{ $t('organDonation.reviewYourDecision.withdraw.body') }}
    </p>
  </div>
</template>

<script>
export default {
  name: 'Withdrawing',
};
</script>

<style module lang="scss" scoped>
  @import '../../style/organdonation';
</style>
