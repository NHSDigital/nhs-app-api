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
        .deleteV1Session()
        .then(() => {
          localStorage.clear();
          sessionStorage.clear();
          window.location.href = '/login';
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
