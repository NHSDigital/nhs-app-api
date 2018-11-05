<template>
  <div :class="$style.loginMain">
    <h2>{{ $t('login.desc') }}</h2>
    <form :action="authoriseUrl" method="get">
      <input :value="scope" type="hidden" name="scope">
      <input v-model="clientId" type="hidden" name="client_id">
      <input :value="codeChallenge" type="hidden" name="code_challenge">
      <input :value="codeChallengeMethod" type="hidden" name="code_challenge_method">
      <input :value="redirectUri" type="hidden" name="redirect_uri">
      <input :value="state" type="hidden" name="state">
      <input :value="responseType" type="hidden" name="response_type">
      <LoginButton />
    </form>
    <div :class="$style.appVersion">
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
import LoginButton from '@/components/LoginButton';
import { BEGINLOGIN } from '@/lib/routes';

export default {
  head() {
    return {
      title: 'Login Screen',
    };
  },
  layout: 'login',
  components: {
    LoginButton,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN.path,
      scope: '',
      codeChallenge: '',
      codeChallengeMethod: '',
      redirectUri: '',
      state: '',
      responseType: '',
      clientId: '',
    };
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
      NativeCallbacks.hideWhiteScreen();
      NativeCallbacks.hideHeaderSlim();
    }

    const authorisationService = new AuthorisationService(this.$env);
    const loginValues = authorisationService.generateLoginValues(
      this.$route.query.source,
      this.$cookies,
    );

    this.authoriseUrl = loginValues.authoriseUrl;
    this.scope = loginValues.scope;
    this.codeChallenge = loginValues.codeChallenge;
    this.codeChallengeMethod = loginValues.codeChallengeMethod;
    this.redirectUri = loginValues.redirectUri;
    this.state = loginValues.state;
    this.responseType = loginValues.responseType;
    this.clientId = loginValues.clientId;
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/home";
.appVersion {
  text-align: center;
  color: #637683;
  font-size: small;
}
</style>
