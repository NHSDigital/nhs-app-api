<template>
  <div id="maincontent" :class="dynamicStyle('loginMain')">
    <div>
      <h2 v-if="isUsingNativeApp" id="native-header"
          class="nhsuk-u-margin-bottom-2">
        {{ $t('login.desc') }}
      </h2>
      <form ref="loginForm"
            :action="instructionsUrl"
            method="get">
        <input :value="redirectTo"
               type="hidden"
               :name="redirectName">
        <generic-button id="viewInstructionsButton"
                        :button-classes="getButtonClasses"
                        @click="onContinueClicked">
          {{ $t('loginButton.login') }}
        </generic-button>
      </form>
    </div>
    <div v-if="isUsingNativeApp"
         :class="[$style['nhsuk-body-s'], $style['appVersion']]">
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </div>
  </div>
</template>
<script>
import AuthorisationService from '@/services/authorisation-service';
import NativeCallbacks from '@/services/native-app';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { BEGINLOGIN, REDIRECT_PARAMETER, PRE_REGISTRATION_INFORMATION } from '@/lib/routes';
import GenericButton from '@/components/widgets/GenericButton';

export default {
  layout: 'login',
  components: {
    GenericButton,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN.path,
      redirectTo: this.$route.query[REDIRECT_PARAMETER],
      redirectName: REDIRECT_PARAMETER,
      instructionsUrl: PRE_REGISTRATION_INFORMATION.path,
      isUsingNativeApp: this.$store.state.device.isNativeApp,
      hasSeenInstructions: this.$store.getters['preRegistrationInformation/instructionsViewed'],
    };
  },
  computed: {
    shouldShowBiometrics() {
      return this.$store.app.$env.BIOMETRICS_ENABLED && this.$store.state.device.isNativeApp;
    },
    getButtonClasses() {
      return ['nhsuk-login', 'nhsuk-body', 'nhsuk-button',
        this.isUsingNativeApp ? 'button' : ''];
    },
  },
  created() {
    this.$store.dispatch('preRegistrationInformation/sync');
  },
  mounted() {
    sessionStorage.clear();
    if (this.shouldShowBiometrics && this.$route.query.fidoAuthResponse) {
      const { authoriseUrl, loginUrl } = this.generateRedirectData();
      this.authoriseUrl = authoriseUrl;
      this.$store.app.context.redirect(loginUrl);
      this.$store.dispatch('analytics/satelliteTrack', 'fidoLogin');
      return;
    }

    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideElements();
    }
  },
  methods: {
    generateRedirectData() {
      const authorisationService = new AuthorisationService(this.$store.app.$env);
      const { request, loginUrl } = authorisationService.generateLoginUrl({
        isNativeApp: this.isUsingNativeApp,
        cookies: this.$cookies,
        redirectTo: this.redirectTo,
        fidoAuthResponse: this.$route.query.fidoAuthResponse,
      });
      return { authoriseUrl: request.authoriseUrl, loginUrl };
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    onContinueClicked() {
      if (!this.isButtonDisabled
        && (!this.isUsingNativeApp
          || this.hasSeenInstructions)) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        this.isButtonDisabled = true;
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/accessibility";
@import '../style/colours';
@import "../style/home";
.appVersion {
  text-align: center;
}
</style>
