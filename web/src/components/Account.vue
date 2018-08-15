<template>
  <div class="content">
    <button id="signout-button" :class="[$style.button, $style.grey]" @click="signoutClicked()">
      {{ $t('signOutButton.signOut') }}
    </button>
  </div>
</template>

<script>

function signoutClicked() {
  this.$store.dispatch('auth/logout');
}

export default {
  methods: {
    signoutClicked,
  },
};
</script>

<style module lang="scss" scoped>
  @import '../style/buttons';
</style>
