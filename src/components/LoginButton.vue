<template>
  <button data-id="login-button" class="button green" v-on:click="loginClicked()">
    {{ $t('loginOrRegister.login') }}
  </button>
</template>

<script>
import { createVerifier, performLogin } from '@/services/authorization-service';

const calculateRedirectUri = () => `${window.location.origin}/auth-return`;

const loginClicked = () => {
  // TODO: Storage of verifier
  // The verifier needs to be stored in a cookie or local storage which is why it is generated
  // here instead of in the authorization service.  The storage of the verifier is outside the
  // scope of this ticket.
  const verifier = createVerifier();
  const redirectUri = process.env.CID_REDIRECT_URI || calculateRedirectUri();
  performLogin(redirectUri, verifier);
};

export default {
  methods: {
    loginClicked,
  },
};
</script>
<style lang="scss" scoped>
  @import '../style/colours';
  @import '../style/textstyles';
  @import '../style/fonts';
  @import '../style/buttons';
  @import '../style/icons';
</style>
