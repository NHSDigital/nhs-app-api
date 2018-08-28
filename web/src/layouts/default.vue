<template>
  <div id="app">
    <home-header v-if="showLoginHeader" />
    <header-menu v-if="showMenu"/>
    <main :class="mainClass">
      <spinner />
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </main>
    <navigation-menu v-if="showMenu"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
/* eslint-disable no-underscore-dangle */
import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import HomeHeader from '@/components/HomeHeader';
import Routes from '../Routes';

export default {
  components: {
    NavigationMenu,
    HeaderMenu,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    HomeHeader,
  },
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - NHS App`,
      script: [
        {
          src: process.env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };

    if (process.env.HOTJAR_FILENAME) {
      head.script.push({ src: `hotjar/${process.env.HOTJAR_FILENAME}.js` });
    }
    return head;
  },
  computed: {
    showMenu() {
      return (
        !this.$store.state.device.isNativeApp &&
        this.$store.state.auth.loggedIn &&
        this.$route.name !== 'Login'
      );
    },
    mainClass() {
      if (this.isLoginPage()) {
        return this.$style.homeMain;
      }
      const clazzes = ['content', 'pull-body'];
      if (this.$store.state.device.isNativeApp) {
        clazzes.push('native');
      }
      return clazzes;
    },
    showLoginHeader() {
      return this.isLoginPage();
    },
  },
  created() {
    if (this.$route.query.source === 'android' || this.$route.query.source === 'ios') {
      this.$store.dispatch('device/updateIsNativeApp', true);
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
    }
    this.$store.dispatch('device/setSourceDevice', this.$route.query.source);
  },
  mounted() {
    this.$store.dispatch('session/startValidationChecking');

    if (process.client && !window.validateSession) {
      window.validateSession = () => {
        this.$store.dispatch('session/validate');
      };
    }
  },
  methods: {
    isLoginPage() {
      return this.$route.name === Routes.LOGIN.name;
    },
  },
};
</script>

<style lang="scss">
@import "../style/main";
@import "../style/pulltorefresh";
@import "../style/elements";
</style>

<style module lang="scss" scoped>
@import "../style/home";
</style>
