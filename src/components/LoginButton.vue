<template>
  <button data-id="login-button" class="button green" v-on:click="loginClicked()">
    {{ $t('loginButton.login') }}
  </button>
</template>

<script>

function loginClicked() {
  this.$store.dispatch('auth/login', this.$config);
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
