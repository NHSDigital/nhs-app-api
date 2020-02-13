<template>
  <div id="maincontent" :class="dynamicStyle('loginMain')">
    <div>
      <h2 class="nhsuk-u-margin-bottom-2">{{ $t('login.desc') }}</h2>
      <form ref="loginForm"
            :action="authoriseUrl"
            method="get">
        <input :value="redirectTo" type="hidden" :name="redirectName">
        <generic-button
          id="login-button"
          :button-classes="['nhsuk-login', 'nhsuk-body', 'nhsuk-button',
                            $store.state.device.isNativeApp
                              ?'button':'']"
          type="submit"
          data-id="login-button"
          @click="trackLogin">
          {{ $t('loginButton.login') }}
        </generic-button>
      </form>
    </div>
    <div v-if="this.$store.state.device.isNativeApp"
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
import GenericButton from '@/components/widgets/GenericButton';
import NativeCallbacks from '@/services/native-app';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { BEGINLOGIN, REDIRECT_PARAMETER } from '@/lib/routes';

export default {
  layout: 'login',
  components: {
    GenericButton,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN.path,
      isButtonDisabled: false,
      redirectTo: this.$route.query[REDIRECT_PARAMETER],
      redirectName: REDIRECT_PARAMETER,
    };
  },
  computed: {
    shouldShowBiometrics() {
      return this.$store.app.$env.BIOMETRICS_ENABLED && this.$store.state.device.isNativeApp;
    },
  },
  mounted() {
    sessionStorage.removeItem('hasAgreedToMedicalWarning');
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
        isNativeApp: this.$store.state.device.isNativeApp,
        cookies: this.$cookies,
        redirectTo: this.redirectTo,
        fidoAuthResponse: this.$route.query.fidoAuthResponse,
      });
      return { authoriseUrl: request.authoriseUrl, loginUrl };
    },
    trackLogin() {
      if (!this.isButtonDisabled) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        this.isButtonDisabled = true;
      }
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
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
