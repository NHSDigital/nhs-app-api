<template>
  <div :class="$style.homeMain">
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
  </div>
</template>
<script>
import AuthorisationService from '@/services/authorisation-service';
import LoginButton from '@/components/LoginButton';

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
  asyncData({ env, app: { $cookies: cookies }, route: { query } }) {
    const authorisationService = new AuthorisationService(env);
    const source = query.source || 'web';

    const loginValues = authorisationService.generateLoginValues({ device: { source } });

    cookies.set('nhso.auth', {
      redirectUri: loginValues.redirectUri,
      codeVerifier: loginValues.codeVerifier,
    });

    return Promise.resolve(loginValues);
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/home";

</style>
