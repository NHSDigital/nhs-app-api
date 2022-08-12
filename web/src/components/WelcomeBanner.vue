<!-- On index route, either hero banner or smaller section is displayed at any time based on css width -->
<template>
  <div v-if="shouldShowWelcomeSection" :class="[$style['welcomeInfo'], $style['webheader-blue'], 'nhsuk-u-margin-bottom-4']">
    <div :class="['nhsuk-width-container']">
      <welcome-section v-if="!isProxying"
                       id="welcome-section"
                       :display-name="currentProfile.name"
                       :nhs-number="currentProfile.nhsNumber" />
      <proxy-welcome-section v-else
                             id="proxy-welcome-section"
                             :proxy-age="proxyAge"
                             :proxy-details="currentProfile"/>
    </div>
  </div>
  <div v-else :class="[$style['heroBanner'], 'nhsuk-u-margin-bottom-8']">
    <picture-banner v-if="!isProxying"
                    id="picture-banner-section"
                    :display-name="currentProfile.name"
                    :nhs-number="currentProfile.nhsNumber" />
    <picture-banner v-else
                    id="proxy-picture-banner-section"
                    :display-name="currentProfile.fullName"
                    :age="proxyAge"
                    :practice="currentProfile.gpPracticeName" />
  </div>
</template>

<script>
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import DefinedScreenSize from '@/lib/screen-sizes';
import PictureBanner from '@/components/PictureBanner';
import ProxyWelcomeSection from '@/components/ProxyWelcomeSection';
import WelcomeSection from '@/components/WelcomeSection';

export default {
  name: 'WelcomeBanner',
  components: {
    PictureBanner,
    ProxyWelcomeSection,
    WelcomeSection,
  },
  mixins: [
    CalculateAgeInMonthsAndYears,
  ],
  data() {
    return {
      isSmallScreen: false,
    };
  },
  computed: {
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
    currentProfile() {
      if (this.isProxying) {
        return this.$store.state.linkedAccounts.actingAsUser;
      }
      return this.$store.getters['session/currentProfile'];
    },
    proxyAge() {
      return this.getDisplayedAgeText(this.currentProfile);
    },
    shouldShowWelcomeSection() {
      return this.isSmallScreen;
    },
  },
  created() {
    // As this is a max and there is only a matches not < than, need to -1 at this point
    this.smallScreenSizeMediaQuery = window.matchMedia(`(max-width: ${DefinedScreenSize.Tablet - 1}px)`);

    this.setIsSmallScreen(this.smallScreenSizeMediaQuery);

    this.smallScreenSizeMediaQuery.addEventListener('change', this.setIsSmallScreen);
  },
  beforeDestroy() {
    this.smallScreenSizeMediaQuery.removeEventListener('change', this.setIsSmallScreen);
  },
  methods: {
    setIsSmallScreen(x) {
      this.isSmallScreen = x.matches;
    },
  },
};
</script>
<style lang="scss" module scoped>
  @import "@/style/custom/home-banner";
</style>
