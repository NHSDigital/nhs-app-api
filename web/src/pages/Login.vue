<template>
  <login-layout>
    <div :class="dynamicStyle('loginMain')">
      <div>
        <h2 v-if="isUsingNativeApp" id="native-header"
            class="nhsuk-u-margin-bottom-2 nhsuk-u-padding-left-0 nhsuk-u-padding-right-0">
          {{ $t('login.toAccessYourNhsServices') }}
        </h2>
        <ul :class="$style['continueWithNhsLogin']"
            :style="[isUsingNativeApp ? {'text-align': 'center'} : {'text-align' : 'left'} ]">
          <li>
            <a id="viewInstructionsButton"
               tabindex="0"
               :aria-label="$t('login.continueWithNhsLogin')"
               href="#"
               @click.stop.prevent="onContinueClicked">
              <img :alt="$t('login.continueWithNhsLogin')"
                   src="../assets/continue-with-nhs-login.png">
            </a>
          </li>
        </ul>
        <br>
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
import NativeApp from '@/services/native-app';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { BEGINLOGIN_PATH, PRE_REGISTRATION_INFORMATION_PATH } from '@/router/paths';
import { REDIRECT_PARAMETER } from '@/router/names';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'LoginPage',
  components: {
    LoginLayout,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN_PATH,
      instructionsUrl: PRE_REGISTRATION_INFORMATION_PATH,
      isButtonDisabled: false,
      isUsingNativeApp: this.$store.state.device.isNativeApp,
      redirectName: REDIRECT_PARAMETER,
      redirectParameter: this.$route.query[REDIRECT_PARAMETER],
    };
  },
  computed: {
    getButtonClasses() {
      return ['nhsuk-login', 'nhsuk-body', 'nhsuk-button',
        this.isUsingNativeApp ? 'button' : ''];
    },
  },
  created() {
    this.$store.dispatch('pageLeaveWarning/reset');

    if (typeof window === 'object') {
      window.onbeforeunload = null;

      if (this.$store.state.device.isNativeApp) {
        NativeApp.dismissPageLeaveWarningDialogue();
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
      NativeApp.hideElements();
    }
  },
  methods: {
    generateRedirectData() {
      const authorisationService = new AuthorisationService(this.$store.$env);
      const { request, loginUrl } = authorisationService.generateLoginUrl({
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
        this.isButtonDisabled = true;
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        redirectTo(this, this.instructionsUrl, this.$route.query);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/login";
</style>
