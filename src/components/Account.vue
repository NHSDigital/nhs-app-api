<template>
  <div id="mainDiv">
    <main class="content">
    <button class="button grey" id="signout-button" v-on:click="signoutClicked()">
      {{ $t('signOutButton.signOut') }}
    </button>
    </main>
  </div>
</template>

<script>
import Vue from 'vue';

function signoutClicked() {
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
