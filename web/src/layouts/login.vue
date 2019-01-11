<template>
  <div v-if="isErrorVisible" :class="$style['error-container']">
    <connection-error />
    <api-error />
  </div>
  <div v-else id="app" :class="$style['login-app-header']">
    <div :class="$style['login-app-header-flex-container']">
      <home-header />
      <session-expired-banner v-if="showSessionExpiredBanner" />
      <main :class="this.$style.homeMain">
        <flash-message />
        <nuxt />
      </main>
    </div>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import HomeHeader from '@/components/HomeHeader';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';
import Sources from '@/lib/sources';

export default {
  components: {
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    HomeHeader,
    SessionExpiredBanner,
  },
  computed: {
    showSessionExpiredBanner() {
      return this.$store.state.session.showExpiryMessage;
    },
    isErrorVisible() {
      return this.$store.getters['errors/showApiError'] || this.$store.state.errors.hasConnectionProblem;
    },
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} screen`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
          async: true,
        },
      ],

    };
  },
  mounted() {
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
  },
  created() {
    if (Sources.isNative(this.$route.query.source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
      this.$store.dispatch('device/setSourceDevice', this.$route.query.source);
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
@import "../style/spacings";

.error-container {
  @include space(padding, all, 1em);
}

.login-app-header {
  position: absolute;
  left:0;
  top:0;
  right:0;
  bottom:0;
}

.login-app-header-flex-container {
  display:flex;
  flex-direction:column;
  height:100%;
}
</style>
