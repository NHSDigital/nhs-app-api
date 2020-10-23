<template>
  <login-layout>
    <div :class="dynamicStyle('loginMain')">
      <div>
        <h2 v-if="isUsingNativeApp" id="native-header"
            class="nhsuk-u-margin-bottom-2 nhsuk-u-padding-left-0 nhsuk-u-padding-right-0">
          {{ $t('login.toAccessYourNhsServices') }}
        </h2>
        <generic-button id="viewInstructionsButton"
                        :button-classes="getButtonClasses"
                        @click="onContinueClicked">
          {{ $t('login.continueWithNhsLogin') }}
        </generic-button>
      </div>
      <div v-if="isUsingNativeApp"
           :class="[$style['nhsuk-body-s'], $style['appVersion']]">
        <span style="word-break:break-word;">
          {{ $t('generic.version') }} {{ this.$store.state.appVersion.webVersion }}
        </span>
        <span v-if="this.$store.state.appVersion.nativeVersion">
          ({{ this.$store.state.appVersion.nativeVersion }})
        </span>
      </div>
    </div>
  </login-layout>
</template>
<script>
import AuthorisationService from '@/services/authorisation-service';
import LoginLayout from '@/layouts/login';
import NativeCallbacks from '@/services/native-app';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { BEGINLOGIN_PATH, PRE_REGISTRATION_INFORMATION_PATH } from '@/router/paths';
import { REDIRECT_PARAMETER } from '@/router/names';
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'LoginPage',
  components: {
    GenericButton,
    LoginLayout,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN_PATH,
      redirectParameter: this.$route.query[REDIRECT_PARAMETER],
      redirectName: REDIRECT_PARAMETER,
      instructionsUrl: PRE_REGISTRATION_INFORMATION_PATH,
      isUsingNativeApp: this.$store.state.device.isNativeApp,
      hasSeenInstructions: this.$store.getters['preRegistrationInformation/instructionsViewed'],
    };
  },
  computed: {
    getButtonClasses() {
      return ['nhsuk-login', 'nhsuk-body', 'nhsuk-button',
        this.isUsingNativeApp ? 'button' : ''];
    },
  },
  created() {
    this.$store.dispatch('preRegistrationInformation/sync');

    this.$store.dispatch('pageLeaveWarning/reset');

    if (typeof window === 'object') {
      window.onbeforeunload = null;

      if (this.$store.state.device.isNativeApp) {
        NativeCallbacks.dismissPageLeaveWarningDialogue();
      }
    }
  },
  mounted() {
    sessionStorage.clear();
    if (this.isUsingNativeApp && this.$route.query.fidoAuthResponse) {
      const { authoriseUrl, loginUrl } = this.generateRedirectData();
      this.authoriseUrl = authoriseUrl;
      this.$store.dispatch('analytics/satelliteTrack', 'fidoLogin');
      window.location = loginUrl;
      return;
    }

    if (this.isUsingNativeApp) {
      NativeCallbacks.hideElements();
    }
  },
  methods: {
    generateRedirectData() {
      const authorisationService = new AuthorisationService(this.$store.$env);
      const { request, loginUrl } = authorisationService.generateLoginUrl({
        isNativeApp: this.isUsingNativeApp,
        cookies: this.$cookies,
        redirectTo: this.redirectParameter,
        fidoAuthResponse: this.$route.query.fidoAuthResponse,
      });
      return { authoriseUrl: request.authoriseUrl, loginUrl };
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    onContinueClicked() {
      if (!this.isButtonDisabled) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        this.isButtonDisabled = true;
        redirectTo(this, this.instructionsUrl, this.redirectParameter);
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
