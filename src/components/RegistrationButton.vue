<template>
  <button data-id="create-account-button" class="create-account-button button"
          @click="createAccountClicked($event)">
    {{ $t('loginOrRegister.createAccount') }}
  </button>
</template>

<script>
export default {
  methods: {
    async createAccountClicked(evt) {
      evt.preventDefault();
      this.$store.dispatch('auth/register', process.env);
    },
  },
};
</script>
<style lang="scss" scoped>
  @import '../style/buttons';
</style>

