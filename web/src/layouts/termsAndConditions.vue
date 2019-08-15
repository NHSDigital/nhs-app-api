<template>
  <div id="app">
    <main :class="[this.$style.homeMain, !$store.state.device.isNativeApp && $style.desktopWeb]">
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </main>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import NativeVersionSetup from '../services/nativeVersionSetup';


export default {
  components: {
    ApiError,
    ConnectionError,
    FlashMessage,
  },
  head() {
    let head = {};
    if (this.$store.state.termsAndConditions.analyticsCookieAccepted) {
      head = {
        htmlAttrs: {
          lang: `${this.$t('language')}`,
        },
        title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
        __dangerouslyDisableSanitizers: ['noscript'],
        script: [],
      };
      if (this.$store.app.$env.ANALYTICS_SCRIPT_URL !== 'NOT_SET') {
        head.script = [
          {
            src: this.$store.app.$env.ANALYTICS_SCRIPT_URL,
          },
        ];
      }
    } else {
      head = {
        htmlAttrs: {
          lang: `${this.$t('language')}`,
        },
        title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
        __dangerouslyDisableSanitizers: ['noscript'],
      };
    }

    const sessionCookie = this.$store.app.$cookies.get('nhso.session');

    if (sessionCookie) {
      const { durationSeconds } = sessionCookie;

      if (durationSeconds) {
        head.noscript = [
          { innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};URL='/account/signout'">`, body: false },
        ];
      }
    }

    return head;
  },
  mounted() {
    NativeVersionSetup(this.$store, this.$route);
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
  },
  created() {
    if (process.browser) {
      this.$store.dispatch('session/updateLastCalledAt');
    }

    const appVersion = this.$store.app.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
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

.homeMain {
  &.desktopWeb {
    padding: 0;
  }
}


</style>
