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
/* eslint-disable import/extensions */
import SymptomBanner from '@/components/SymptonBanner';
import NhsLogo from '@/components/icons/NhsLogo';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';
import Routes from '@/Routes';

export default {
  components: {
    SymptomBanner,
    NhsLogo,
    SessionExpiredBanner,
  },
  computed: {
    showSessionExpiredBanner() {
      const isLoginPage = this.$route.name === Routes.LOGIN.name;
      return isLoginPage && this.$store.state.session.showExpiryMessage;
    },
  },
};
</script>

<style module lang="scss"  scoped>
@import "../style/homeheader";

</style>
