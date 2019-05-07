<template>
  <div id="app" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="!$store.state.device.isNativeApp" :class="$style['header-container-desktop']">
      <web-header :show-menu="false" :show-links="false"/>
    </div>
    <div v-else>
      <header-slim :show-in-native="true" :click-url="loginUrl">{{ headerTitle }}</header-slim>
    </div>
    <div :class="[mainClass, $style['main-container-desktop']]">
      <main :class="mainClass">
        <spinner />
        <connection-error />
        <api-error />
        <flash-message />
        <nuxt />
      </main>
    </div>
    <div v-if="!$store.state.device.isNativeApp" :class="$style['footer-container-desktop']">
      <web-footer/>
    </div>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import Sources from '@/lib/sources';
import HeaderSlim from '@/components/HeaderSlim';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import { LOGIN } from '@/lib/routes';
import NativeVersionSetup from '../services/nativeVersionSetup';

export default {
  components: {
    HeaderSlim,
    WebHeader,
    WebFooter,
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
      const classes = ['content', 'pull-body'];
      if (this.$store.state.device.isNativeApp) {
        classes.push('native');
        classes.push('slim');
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
  mounted() {
    NativeVersionSetup(this.$store, this.$route);
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
      return this.hasApiError || this.hasConnectionError; // API or connection errors
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

 div {
  &.desktopWeb {
   display: flex;
   flex-direction: column;
   flex-wrap: nowrap;
   justify-content: flex-start;
   align-content: stretch;
   align-items: flex-start;
   min-height: 100vh;

   .header-container-desktop, .footer-container-desktop {
    order: 0;
    flex: 0 0 auto;
    align-self: stretch;
   }

   .main-container-desktop {
    order: 0;
    flex: 1 0 auto;
    align-self: stretch;
   }
  }
 }
</style>
