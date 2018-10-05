<template>
  <div id="app">
    <header-menu :show-account-icon="false"/>
    <main :class="mainClass">
      <spinner />
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </main>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import HeaderMenu from '@/components/HeaderMenu';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import Sources from '@/lib/sources';

export default {
  components: {
    HeaderMenu,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
  },
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - NHS App`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };
    return head;
  },
  computed: {
    mainClass() {
      const classes = ['content', 'pull-body'];
      if (this.$store.state.device.isNativeApp) {
        classes.push('native');
      }
      return classes;
    },
  },
  created() {
    if (Sources.isNative(this.$route.query.source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
    }
    this.$store.dispatch('device/setSourceDevice', this.$route.query.source);
  },
  methods: {
    pageTitle() {
      const nhsApp = 'NHS App';

      if (this.$store.state.pageTitle.pageTitle) {
        return `${this.$store.state.pageTitle.pageTitle}-${nhsApp}`;
      }

      return nhsApp;
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
