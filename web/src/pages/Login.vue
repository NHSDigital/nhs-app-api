<template>
  <div :class="$style.homeMain">
    <h2>{{ $t('login.desc') }}</h2>
    <form :action="loginUrl" method="get">
      <input :value="scope" type="hidden" name="scope">
      <input v-model="clientId" type="hidden" name="client_id">
      <input :value="codeChallenge" type="hidden" name="code_challenge">
      <input :value ="codeMethod" type="hidden" name="code_challenge_method">
      <input :value="redirectUri" type="hidden" name="redirect_uri">
      <input :value="state" type="hidden" name="state">
      <input :value="responseType" type="hidden" name="response_type">
      <LoginButton />
    </form>
  </div>
</template>
<script>
import AuthorisationService from '@/services/authorization-service';
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
  asyncData({ app: { $cookies: cookies }, route: { query } }) {
    const source = query.source || 'web';
    const codeVerifier = AuthorisationService.createVerifier();
    const {
      baseUrl: loginUrl,
      client_id: clientId,
      code_challenge: codeChallenge,
      code_challenge_method: codeMethod,
      state,
      prompt,
      registerUrl,
      redirect_uri: redirectUri,
      response_type: responseType,
      scope,
    } = new AuthorisationService().buildLoginObject(codeVerifier, { device: { source } });

    cookies.set('nhso.auth', {
      redirectUri,
      codeVerifier,
    });

    return Promise.resolve({
      codeVerifier,
      loginUrl,
      clientId,
      codeChallenge,
      codeMethod,
      state,
      prompt,
      registerUrl,
      redirectUri,
      responseType,
      scope,
    });
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/home";

</style>
