<template>
  <content>
    <home-header />
    <SessionExpiredBanner />
    <main class="login-or-register">
      <form :action="redirectUrl" method="get">
        <input :value="clientId" type="hidden" name="client_id" >
        <input :value="codeChallenge" type="hidden" name="code_challenge">
        <input :value ="codeMethod" type="hidden" name="code_challenge_method" >
        <input :value="prompt" type="hidden" name="prompt">
        <input :value="redirectUri" type="hidden" name="redirect_uri">
        <input :value="state" type="hidden" name="state">
        <input :value="responseType" type="hidden" name="response_type">
        <LoginButton />
      </form>
      <hr :data-content="$t('common.or')" class="hr-text">
      <form :action="registerUrl" method="get">
        <input :value="redirectUri" type="hidden" name="redirect_uri">
        <input :value="clientId" type="hidden" name="client_id" >
        <input :value="codeChallenge" type="hidden" name="code_challenge">
        <input :value ="codeMethod" type="hidden" name="code_challenge_method" >
        <input :value="responseType" type="hidden" name="response_type">
        <input :value="state" type="hidden" name="state">
        <RegistrationButton />
      </form>
    </main>
  </content>
</template>
<script>
/* eslint-disable import/extensions */
import LoginButton from '@/components/LoginButton';
import HomeHeader from '@/components/HomeHeader';
import RegistrationButton from '@/components/RegistrationButton';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';

export default {
  middleware: ['meta', 'buildAuth'],
  head() {
    return {
      title: 'Login Screen',
    };
  },
  components: {
    RegistrationButton,
    LoginButton,
    HomeHeader,
    SessionExpiredBanner,
  },
  computed: {
    redirectUrl() {
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
    state() {
      return this.$store.state.auth.config.state;
    },
    prompt() {
      return this.$store.state.auth.config.prompt;
    },
    redirectUri() {
      return this.$store.state.auth.config.redirect_uri;
    },
    responseType() {
      return this.$store.state.auth.config.response_type;
    },
  },
};
</script>
<style lang="scss"  scoped>
@import "../style/colours";
@import "../style/textstyles";
@import "../style/fonts";
@import "../style/buttons";
main {
  height: 200px;
  padding: 24px;
  box-sizing: border-box;

  .hr-text {
    font-family: $default;
    line-height: 1em;
    position: relative;
    outline: 0;
    border: 0;
    color: $mid_grey;
    text-align: center;
    height: 1.5em;
    margin-bottom: 16px;

    &:before {
      content: "";
      background: $mid_grey;
      position: absolute;
      left: 0;
      top: 50%;
      width: 100%;
      height: 1px;
    }

    &:after {
      content: attr(data-content);
      position: relative;
      display: inline-block;
      padding: 0 0.5em;
      line-height: 1.5em;
      color: #818078;
      background-color: $white;
    }
  }
}
</style>
