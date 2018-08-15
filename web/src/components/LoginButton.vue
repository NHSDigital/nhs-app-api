<template>
  <button :class="[$style.button, $style.green]"
          data-id="login-button" @click="loginClicked($event)">
    {{ $t('loginButton.login') }}
  </button>
</template>

<script>
export default {
  methods: {
    async loginClicked(evt) {
      evt.preventDefault();
      this.$store.dispatch('header/updateHeaderText', this.$t('pageHeaderTitles.login'));
      this.$store.dispatch('auth/login', process.env);
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/buttons";

</style>
