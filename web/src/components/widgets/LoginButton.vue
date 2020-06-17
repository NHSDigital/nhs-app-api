<template>
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
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import { BEGINLOGIN_PATH } from '@/router/paths';
import {
  LOGIN_NAME,
  PRE_REGISTRATION_INFORMATION_NAME,
  REDIRECT_PARAMETER,
} from '@/router/names';
import { redirectTo } from '@/lib/utils';

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
      authoriseUrl: BEGINLOGIN_PATH,
      redirectTo: this.$route.query[REDIRECT_PARAMETER],
    };
  },
  computed: {
    isLoginPage() {
      return this.$route.name === LOGIN_NAME;
    },
  },
  methods: {
    async trackLogin() {
      if (!this.isButtonDisabled) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        this.isButtonDisabled = true;
        if (this.$route.name === PRE_REGISTRATION_INFORMATION_NAME) {
          await this.$store.dispatch('preRegistrationInformation/continue');
        }
        redirectTo(this, this.authoriseUrl, this.redirectTo);
      }
    },
  },
};
</script>
