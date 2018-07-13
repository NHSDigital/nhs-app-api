<template>
  <div id="app" :class="{ menusVisible: showMenu}">
    <header-menu v-if="showMenu"/>
    <div id="mainDiv">
      <spinner />
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </div>
    <navigation-menu v-if="showMenu"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';

export default {
  components: {
    NavigationMenu,
    HeaderMenu,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} screen`,
    };
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
  created() {
    if (this.$route.query.source === 'mobile') {
      this.$store.dispatch('device/updateIsNativeApp', true);
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
    }
  },
  mounted() {
    this.$store.dispatch('session/startValidationChecking');

    if (process.client && !window.validateSession) {
      window.validateSession = () => {
        this.$store.dispatch('session/validate');
      };
    }
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
