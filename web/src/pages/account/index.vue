<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate"
       :class="[$style['no-padding'],
                'pull-content', !$store.state.device.isNativeApp && $style.desktopWeb]">
    <h2>{{ $t('myAccount.detailsHeading') }}</h2>
    <div :class="$style.welcomeSectionContainer">
      <welcome-section :name="$store.state.session.user"
                       :date-of-birth="$store.state.session.dateOfBirth"
                       :nhs-number="$store.state.session.nhsNumber" />
    </div>
    <settings v-if="showBiometrics || showNotifications"
              :show-notifications="showNotifications"
              :show-biometrics="showBiometrics"/>
    <about-us/>
    <p>
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </p>
    <p v-if="this.$store.app.$env.CE_MARK_ENABLED">
      <ce-mark-icon/>
    </p>
    <analytics-tracked-tag :text="$t('signOutButton.signOut')"
                           data-purpose="button">
      <form action="account/signout" method="post">
        <floating-button-bottom
          v-if="$store.state.device.isNativeApp"
          id="signout-button" type="submit"
          @click="signout">
          {{ $t('signOutButton.signOut') }}
        </floating-button-bottom>
      </form>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AboutUs from '@/components/account/AboutUs';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CeMarkIcon from '@/components/icons/CeMarkIcon';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import NativeCallbacks from '@/services/native-app';
import Settings from '@/components/account/Settings';
import sjrIf from '@/lib/sjrIf';
import WelcomeSection from '@/components/WelcomeSection';

export default {
  components: {
    AboutUs,
    AnalyticsTrackedTag,
    CeMarkIcon,
    FloatingButtonBottom,
    Settings,
    WelcomeSection,
  },
  data() {
    return {
      nativeLoginOptionsMethodExists: true,
    };
  },
  computed: {
    showBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.nativeLoginOptionsMethodExists &&
        this.$store.state.device.isNativeApp;
    },
    showNotifications() {
      return sjrIf({ $store: this.$store, journey: 'notifications' }) &&
        this.$store.state.device.isNativeApp;
    },
  },
  mounted() {
    this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
  },
  methods: {
    signout(event) {
      event.preventDefault();
      this.$store.dispatch('auth/logout');
    },
    goToLoginOptions() {
      NativeCallbacks.goToLoginOptions();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/accessibility";
@import "../../style/listmenu";
@import "../../style/colours";
@import "../../style/webshared";

</style>
