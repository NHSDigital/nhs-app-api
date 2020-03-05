<template>
  <div v-if="showTemplate">
    <h2 data-purpose="greeting" data-hj-suppress class="nhsuk-u-margin-bottom-0">
      {{ greetingMessage }}
    </h2>
    <welcome-section v-if="!isProxying" :date-of-birth="currentProfile.dateOfBirth"
                     :nhs-number="currentProfile.nhsNumber" />
    <proxy-welcome-section v-else :proxy-age="proxyAge" :proxy-details="currentProfile"/>
    <biometric-banner v-if="!isProxying" />
    <navigation-list-menu v-if="!isProxying" />
  </div>
</template>

<script>
import BiometricBanner from '../components/widgets/BiometricBanner';
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import NavigationListMenu from '../components/NavigationListMenu';
import ProxyWelcomeSection from '../components/ProxyWelcomeSection';
import WelcomeSection from '../components/WelcomeSection';

export default {
  layout: 'nhsuk-layout',
  components: {
    BiometricBanner,
    WelcomeSection,
    NavigationListMenu,
    ProxyWelcomeSection,
  },
  mixins: [CalculateAgeInMonthsAndYears],
  computed: {
    currentProfile() {
      if (this.isProxying) {
        return this.$store.state.linkedAccounts.actingAsUser;
      }
      return this.$store.getters['session/currentProfile'];
    },
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      if (this.isProxying) {
        const { fullName } = this.currentProfile;
        return `${message}, ${fullName}`;
      }
      const { name } = this.currentProfile;
      return `${message}, ${name}`;
    },
    proxyAge() {
      return this.getDisplayedAgeText(this.currentProfile);
    },
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
  },
  mounted() {
    window.scrollTo(0, 0);
  },
};
</script>
