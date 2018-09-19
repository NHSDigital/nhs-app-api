<template>
  <div id="app">
    <main :class="this.$style.homeMain">
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </main>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import HomeHeader from '@/components/HomeHeader';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';

export default {
  components: {
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    HomeHeader,
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} screen`,
      script: [
        {
          src: this.$store.app.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };
  },
  mounted() {
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
  },
  created() {
    if (this.$route.query.source === 'android' || this.$route.query.source === 'ios') {
      this.$store.dispatch('device/updateIsNativeApp', true);
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
    }
    this.$store.dispatch('device/setSourceDevice', this.$route.query.source);

    if (process.browser) {
      this.$store.dispatch('session/updateLastCalledAt');
    }
  },
};
</script>

<style lang="scss">
@import "../style/main";
@import "../style/pulltorefresh";
@import "../style/elements";
@import "../style/accessibility";
</style>

<style module lang="scss" scoped>
@import "../style/home";
</style>
