<template>
  <div id="app" :class="{ menusVisible: showMenu}">
    <header-menu v-if="showMenu"/>
    <div id="mainDiv">
      <spinner />
      <connection-error />
      <api-server-error />
      <nuxt v-if="showView" />
    </div>
    <navigation-menu v-if="showMenu"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';
import Spinner from '@/components/Spinner';
import ApiServerError from '@/components/errors/ApiServerError';
import ConnectionError from '@/components/errors/ConnectionError';

export default {
  components: {
    NavigationMenu,
    HeaderMenu,
    Spinner,
    ApiServerError,
    ConnectionError,
  },
  data() {
    return {
      noConnection: false,
    };
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} screen`,
    }
  },
  methods: {
    hasApiServerErrorResponse() {
      const response = this.$store.state.http.apiErrorResponse;
      return (response && response.status >= 500);
    },
  },
  computed: {
    showMenu() {
      return (
        !this.$store.state.device.isNativeApp &&
        this.$store.state.auth.loggedIn &&
        this.$route.name !== 'Login'
      );
    },
    showView() {
      return !this.noConnection && !this.hasApiServerErrorResponse();
    },
  },
  updated() {
    this.noConnection = !navigator.onLine;
  },
  created() {
    if (this.$route.query.source === 'mobile') {
      this.$store.dispatch('device/updateIsNativeApp', true);
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
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
