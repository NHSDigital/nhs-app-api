<template>
  <div :class="!$store.state.device.isNativeApp && $style.mainContentContainer">

    <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content',
                                      $style.mainContent]">
      <div :class="$style['banner-container']">
        <beta-banner data-sid="beta-flag"/>
      </div>

      <hr :class="$style.rule" aria-hidden="true">
      <h2 :class="$style.header" data-purpose="greeting" data-hj-suppress>{{ greetingMessage }}</h2>
      <welcome-section :date-of-birth="$store.state.session.dateOfBirth"
                       :nhs-number="$store.state.session.nhsNumber" />
      <biometric-banner />
      <div :class="$style.navigationList">
        <navigation-list-menu
          nhs-number="nhsNumber"
          dob="dob"
        />
      </div>
    </div>
  </div>

</template>

<script>
/* eslint-disable import/extensions */
import BetaBanner from '../components/BetaBanner';
import WelcomeSection from '../components/WelcomeSection';
import NavigationListMenu from '../components/NavigationListMenu';
import BiometricBanner from '../components/widgets/BiometricBanner';
import NativeCallbacks from '@/services/native-app';
import { accountLinks } from '@/lib/common-links';

export default {
  components: {
    BetaBanner,
    BiometricBanner,
    WelcomeSection,
    NavigationListMenu,
  },
  data() {
    return {
      links: accountLinks(this.$env),
      nativeLoginOptionsMethodExists: true,
    };
  },
  computed: {
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const { user } = this.$store.state.session;
      return `${message}, ${user}`;
    },
    shouldShowDesktopVersion() {
      return (this.$store.state.device.source !== 'android' && this.$store.state.device.source !== 'ios');
    },
    shouldShowBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.nativeLoginOptionsMethodExists &&
        (this.$store.state.device.source === 'android' || this.$store.state.device.source === 'ios');
    },
  },
  mounted() {
    window.scrollTo(0, 0);
    this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/fonts";
  @import "../style/textstyles";
  @import '../style/screensizes';

  .webStyledSpacing {
    height: 0.063em;
    border: none;
    background-color: grey;
    border-top: 1px grey solid;
  }

  .rule {
    height: 0.063em;
    border: none;
    background-color: #D8DDE0;
    border-top: 1px grey solid;
  }

  .banner-container {
    padding: 1em 1em 0.7em;
  }

 div {
  &.mainContentContainer {
   max-width: 960px;
   margin: 0 1em;


   .mainContent {
    display: block;
    max-width: 540px;
    width: auto;
    position: relative;
    transition: opacity 0.2s;
    user-select: none;
    padding-bottom: 2.5em !important;
   }
  }


  @include tablet-and-above {
   .rule {
    margin-right: 0;
    padding-left: 1em;

   }
  }

  @include desktop {
    .header, .navigationList, .mainContentContainer {
      margin: 0 auto;
    }
  }
 }

</style>
