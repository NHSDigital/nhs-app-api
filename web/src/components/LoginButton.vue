<template>
  <button :class="[$style.button, $style.green]" data-id="login-button" @click="loginClicked">
    {{ $t('loginButton.login') }}
  </button>
</template>

<script>
export default {
  methods: {
    async loginClicked() {
      if (process.client) {
        // eslint-disable-next-line no-underscore-dangle
        window._satellite.track('login');
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "../style/buttons";
</style>
