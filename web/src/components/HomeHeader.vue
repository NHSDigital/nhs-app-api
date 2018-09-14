<template>
  <div :class="$style['home-header']">
    <header>
      <div :class="$style.spacer" />
      <nhs-logo/>
      <div :class="$style.spacer" />
      <symptom-banner/>
    </header>
  </div>
</template>

<script>
import SymptomBanner from '@/components/SymptonBanner';
import NhsLogo from '@/components/icons/NhsLogo';

export default {
  components: {
    SymptomBanner,
    NhsLogo,
  },
};
</script>

<style module lang="scss"  scoped>
@import "../style/homeheader";
</style>
