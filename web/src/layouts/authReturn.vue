<template>
  <div id="app">
    <header-slim :show-in-native="true" :click-url="loginUrl">{{ headerTitle }}</header-slim>
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
import HeaderSlim from '@/components/HeaderSlim';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import Sources from '@/lib/sources';
import { LOGIN } from '@/lib/routes';

export default {
  components: {
    HeaderSlim,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
  },
  mixins: [ErrorMessageMixin],
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };
    return head;
  },
  computed: {
    loginUrl() {
      return LOGIN.path;
    },
    headerTitle() {
      return this.showError()
        ? this.getMessage('header')
        : this.$store.state.header.headerText;
    },
    mainClass() {
      const classes = ['content', 'pull-body', 'slim'];
      if (this.$store.state.device.isNativeApp) {
        classes.push('native');
      }
      return classes;
    },
  },
  created() {
    const { source } = this.$route.query;

    if (Sources.isNative(source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
      this.$store.dispatch('device/setSourceDevice', source);
    }
  },
  methods: {
    pageTitle() {
      const nhsApp = 'NHS App';
      const { pageTitle } = this.$store.state.pageTitle;

      if (pageTitle) {
        return `${pageTitle}-${nhsApp}`;
      }

      return nhsApp;
    },
    showError() {
      return this.hasApiError() || this.hasConnectionError(); // API or connection errors
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
