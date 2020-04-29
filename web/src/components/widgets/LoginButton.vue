<template>
  <form ref="loginForm"
        :action="authoriseUrl"
        method="get">
    <input :value="redirectTo" type="hidden" :name="redirectName">
    <generic-button
      id="login-button"
      class="nhsuk-u-margin-bottom-3"
      :button-classes="[isLoginPage ? ['nhsuk-login', 'nhsuk-body'] : '', 'nhsuk-button',
                        isLoginPage && $store.state.device.isNativeApp
                          ?'button':'']"
      type="submit"
      data-id="login-button"
      @click="trackLogin">
      {{ $t(buttonText) }}
    </generic-button>
  </form>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import { BEGINLOGIN,
  REDIRECT_PARAMETER,
  LOGIN,
  PRE_REGISTRATION_INFORMATION } from '@/lib/routes';

export default {
  name: 'LoginButton',
  components: { GenericButton },
  props: {
    buttonText: {
      type: String,
      default: 'loginButton.login',
    },
  },
  data() {
    return {
      isButtonDisabled: false,
      authoriseUrl: BEGINLOGIN.path,
      redirectTo: this.$route.query[REDIRECT_PARAMETER],
      redirectName: REDIRECT_PARAMETER,
    };
  },
  computed: {
    isLoginPage() {
      return this.$route.name === LOGIN.name;
    },
  },
  methods: {
    trackLogin() {
      if (!this.isButtonDisabled) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        this.isButtonDisabled = true;
        if (this.$route.name === PRE_REGISTRATION_INFORMATION.name) {
          this.$store.dispatch('preRegistrationInformation/continue');
        }
      }
    },
  },
};
</script>
