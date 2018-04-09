<template>
  <button data-id="login-button" class="button green" v-on:click="loginClicked()">
    {{ $t('loginButton.login') }}
  </button>
</template>

<script>
import AuthorizationService from '@/services/authorization-service';

function loginClicked() {
  // TODO: Storage of verifier
  // The verifier needs to be stored in a cookie or local storage which is why it is generated
  // here instead of in the authorization service.  The storage of the verifier is outside the
  // scope of this ticket.

  const authorizationService = new AuthorizationService(this.$config);
  const verifier = authorizationService.createVerifier();
  authorizationService.performLogin(verifier);
}

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
