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
import Sources from '@/lib/sources';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import HomeHeader from '@/components/HomeHeader';
import Spinner from '@/components/widgets/Spinner';
import NativeVersionSetup from '../services/nativeVersionSetup';


export default {
  components: {
    ApiError,
    ConnectionError,
    FlashMessage,
    Spinner,
    HomeHeader,
  },
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
      script: [
        {
          src: this.$store.app.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
      __dangerouslyDisableSanitizers: ['noscript'],
    };

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
    if (Sources.isNative(this.$route.query.source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
      this.$store.dispatch('device/setSourceDevice', this.$route.query.source);
    }

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
