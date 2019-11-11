<template>
  <div v-if="showTemplate">
    <h2 data-purpose="greeting" data-hj-suppress>{{ greetingMessage }}</h2>
    <welcome-section :date-of-birth="currentProfile.dateOfBirth"
                     :nhs-number="currentProfile.nhsNumber" />
    <biometric-banner v-if="!isProxying" />
    <navigation-list-menu v-if="!isProxying" />
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import WelcomeSection from '../components/WelcomeSection';
import NavigationListMenu from '../components/NavigationListMenu';
import BiometricBanner from '../components/widgets/BiometricBanner';

export default {
  layout: 'nhsuk-layout',
  components: {
    BiometricBanner,
    WelcomeSection,
    NavigationListMenu,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
    };
  },
  computed: {
    currentProfile() {
      return this.$store.getters['session/currentProfile'];
    },
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const { name } = this.currentProfile;
      return `${message}, ${name}`;
    },
  },
  mounted() {
    window.scrollTo(0, 0);
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/fonts";
  @import "../style/textstyles";
  @import '../style/screensizes';
</style>
