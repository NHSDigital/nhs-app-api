<template>
  <div id="app" :class="{ menusVisible: showMenu}">
    <header-menu v-if="showMenu"/>
    <nuxt/>
    <navigation-menu v-if="showMenu"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';

export default {
  components: {
    NavigationMenu,
    HeaderMenu,
  },
  computed: {
    showMenu() {
      return (
        !this.$store.state.device.isNativeApp &&
        this.$store.state.auth.loggedIn &&
        this.$route.name !== 'Login'
      );
    },
  },
  methods: {
    created() {
      if (this.$route.query.source === 'mobile') {
        this.$store.dispatch('device/updateIsNativeApp', true);
      }
    },
  },
};
</script>

<style lang="scss">
@import "../style/html";
.menusVisible {
  margin-top: 100px;
  margin-bottom: 70px;
}
</style>
