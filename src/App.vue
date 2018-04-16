<template>
  <div id="app" v-bind:class="{ menusVisible: showMenu }">
    <header-menu v-if="showMenu" />
    <router-view/>
    <navigation-menu v-if="showMenu" />
  </div>
</template>

<script>

import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';

export default {
  name: 'App',
  components: {
    NavigationMenu,
    HeaderMenu,
  },

  data() {
    return {
      isNativeApp: false,
      isLoggedIn: false,
    };
  },

  computed: {
    showMenu() {
      return !this.isNativeApp && this.$store.state.auth.loggedIn;
    },
  },

  created() {
    if (this.$route.query.source === 'mobile') {
      this.isNativeApp = true;
    }
  },
};
</script>

<style lang="scss">

  .menusVisible {
    margin-top: 100px;
    margin-bottom: 70px;
  }

</style>
