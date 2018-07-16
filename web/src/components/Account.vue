<template>
  <main class="content">
    <button id="signout-button" class="button grey" @click="signoutClicked($event)">
      {{ $t('signOutButton.signOut') }}
    </button>
  </main>
</template>

<script>

function signoutClicked(evt) {
  evt.preventDefault();
  this.$store.dispatch('auth/logout');
}

export default {
  methods: {
    signoutClicked,
  },
};
</script>

<style lang="scss" scoped>
  @import '../style/html';
  @import '../style/elements';
  @import '../style/buttons';
</style>
