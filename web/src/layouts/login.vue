<template>
  <div id="app" :class="$style['login-app-header']">
    <div :class="$style['login-app-header-flex-container']">
      <home-header />
      <session-expired-banner v-if="showSessionExpiredBanner" />
      <main :class="this.$style.homeMain">
        <connection-error />
        <api-error />
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
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
    }
    this.$store.dispatch('device/setSourceDevice', this.$route.query.source);
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
