<template>
  <div id="app">
    <header-menu v-if="showMenu"/>
    <main :class="mainClass">
      <spinner />
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </main>
    <SurveyBar v-if="onEntryPage"/>
    <navigation-menu v-if="showMenu"/>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SurveyBar from '@/components/SurveyBar';
import Routes from '../Routes';

export default {
  components: {
    NavigationMenu,
    HeaderMenu,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    SurveyBar,
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
        this.loggedIn &&
        this.$route.name !== 'Login'
      );
    },
    onEntryPage() {
      return this.$route.name === 'index';
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
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
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
    if (process.client) {
      if (this.loggedIn) {
        this.$store.dispatch('session/startValidationChecking');
        window.validateSession =
          window.validateSession || (() => this.$store.dispatch('session/validate'));

        this.$store.dispatch('auth/nativeLogin');
      }
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
