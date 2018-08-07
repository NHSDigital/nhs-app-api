<template>
  <div :class="$style['home-header']">
    <header>
      <div :class="$style.spacer" />
      <nhs-logo/>
      <div :class="$style.spacer" />
      <symptom-banner/>
    </header>
    <session-expired-banner v-if="showSessionExpiredBanner" />
  </div>
</template>

<script>
import SymptomBanner from '@/components/SymptonBanner';
import NhsLogo from '@/components/icons/NhsLogo';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';

export default {
  components: {
    SymptomBanner,
    NhsLogo,
    SessionExpiredBanner,
  },
  computed: {
    showSessionExpiredBanner() {
      return this.$store.state.session.showExpiryMessage;
    },
  },
};
</script>

<style module lang="scss"  scoped>
@import "../style/homeheader";
</style>
