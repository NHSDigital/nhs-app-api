<template>
  <main class="content">
    <button id="signout-button" class="button grey" @click="signout()">
      {{ $t('signOutButton.signOut') }}
    </button>
  </main>
</template>

<script>
export default {
  middleware: ['auth', 'meta'],
  methods: {
    signout() {
      this.$store.dispatch('auth/logout', '/login');
    },
  },
};
</script>

<style lang="scss" scoped>
@import "../../style/html";
@import "../../style/elements";
@import "../../style/buttons";
</style>
