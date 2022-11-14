<!-- On index route, either hero banner or smaller section is displayed at any time based on css width -->
<template>
  <div v-if="shouldShowWelcomeSection" :class="[$style['welcomeInfo'], $style['webheader-blue'], 'nhsuk-u-margin-bottom-4']">
    <div :class="['nhsuk-width-container']">
      <welcome-section v-if="!isProxying"
                       id="welcome-section"
                       :display-name="displayNameText"
                       :nhs-number="currentProfile.nhsNumber" />
      <proxy-welcome-section v-else
                             id="proxy-welcome-section"
                             :proxy-age="proxyAge"
                             :proxy-details="currentProfile"
                             :display-name="displayNameText"/>
    </div>
  </div>
  <div v-else :class="[$style['heroBanner'], 'nhsuk-u-margin-bottom-8']">
    <picture-banner v-if="!isProxying"
                    id="picture-banner-section"
                    :display-name="displayNameText"
                    :nhs-number="currentProfile.nhsNumber" />
    <picture-banner v-else
                    id="proxy-picture-banner-section"
                    :display-name="displayNameText"
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
import FormatDisplayName from '@/plugins/mixinDefinitions/FormatDisplayName';

export default {
  name: 'WelcomeBanner',
  components: {
    PictureBanner,
    ProxyWelcomeSection,
    WelcomeSection,
  },
  mixins: [
    CalculateAgeInMonthsAndYears,
    FormatDisplayName,
  ],
  data() {
    const isProxying = this.$store.getters['session/isProxying'];
    const currentProfile = this.$store.getters['session/currentProfile'];
    const displayNameText = isProxying ?
      this.getDisplayNameText(currentProfile.fullName)
      : this.getDisplayNameText(currentProfile.name);
    const proxyAge = isProxying ? this.getDisplayedAgeText(currentProfile) : '';

    return {
      isSmallScreen: false,
      isProxying,
      currentProfile,
      displayNameText,
      proxyAge,
    };
  },
  computed: {
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
