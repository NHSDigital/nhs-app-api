<template>
  <div v-if="showTemplate">
    <h2 data-purpose="greeting" data-hj-suppress>{{ greetingMessage }}</h2>
    <welcome-section :date-of-birth="$store.state.session.dateOfBirth"
                     :nhs-number="$store.state.session.nhsNumber" />
    <biometric-banner />
    <navigation-list-menu
      nhs-number="nhsNumber"
      dob="dob"
    />
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

  computed: {
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const { user } = this.$store.state.session;
      return `${message}, ${user}`;
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
