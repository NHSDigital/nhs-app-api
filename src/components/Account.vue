<template>
  <div id="mainDiv">
    <main class="content">
    <button class="button grey" id="signout-button" v-on:click="singoutClicked()">
      {{ $t('signOutButton.signOut') }}
    </button>
    </main>
  </div>
</template>

<script>
import Vue from 'vue';

export default {
  methods: {
    singoutClicked() {
      Vue
        .$http
        .deleteSession()
        .then(() => {
          this.$router.push('/login');
        });
    },
  },
};
</script>

<style lang="scss" scoped>
  @import '../style/html';
  @import '../style/elements';
  @import '../style/buttons';
</style>
