<template>
  <main v-if="showTemplate" class="content">
    <form action="account/signout" method="get">
      <button id="signout-button" class="button grey" type="submit" @click="signout($event)">
        {{ $t('signOutButton.signOut') }}
      </button>
    </form>
  </main>
</template>

<script>
export default {
  methods: {
    signout(evt) {
      evt.preventDefault();
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
