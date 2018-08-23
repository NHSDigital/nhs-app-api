<template>
  <div :class="$style.homeMain">
    <h2>{{ $t('login.desc') }}</h2>
    <form :action="loginUrl" method="get">
      <input :value="scope" type="hidden" name="scope" >
      <input :value="clientId" type="hidden" name="client_id" >
      <input :value="codeChallenge" type="hidden" name="code_challenge">
      <input :value ="codeMethod" type="hidden" name="code_challenge_method" >
      <input :value="this.$store.state.auth.redirectUri" type="hidden" name="redirect_uri">
      <input :value="state" type="hidden" name="state">
      <input :value="responseType" type="hidden" name="response_type">
      <LoginButton />
    </form>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import LoginButton from '@/components/LoginButton';

export default {
  middleware: ['buildAuth'],
  head() {
    return {
      title: 'Login Screen',
    };
  },
  components: {
    LoginButton,
  },
  computed: {
    loginUrl() {
      return this.$store.state.auth.config.baseUrl;
    },
    registerUrl() {
      return this.$store.state.auth.config.registerUrl;
    },
    clientId() {
      return this.$store.state.auth.config.client_id;
    },
    codeChallenge() {
      return this.$store.state.auth.config.code_challenge;
    },
    codeMethod() {
      return this.$store.state.auth.config.code_challenge_method;
    },
    scope() {
      return this.$store.state.auth.config.scope;
    },
    state() {
      return this.$store.state.auth.config.state;
    },
    responseType() {
      return this.$store.state.auth.config.response_type;
    },
  },
  created() {
    this.$store.dispatch('auth/setRedirectUri');
  },
  mounted() {
    this.$store.dispatch('auth/buildLogin');
  },
};
</script>
<style module lang="scss"  scoped>
@import "../style/home";
</style>
