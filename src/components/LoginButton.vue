<template>
  <button data-id="login-button" class="button green" @click="loginClicked($event)">
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
<style lang="scss" scoped>
@import "../style/colours";
@import "../style/textstyles";
@import "../style/fonts";
@import "../style/buttons";
@import "../style/icons";
</style>
