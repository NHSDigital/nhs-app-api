<template>
  <div id="app">
    <div>
      <web-header :show-menu="false"
                  :show-links="false"
                  :show-header-buttons="false"/>
    </div>
    <content-header id="content-header"
                    :show-bread-crumb="false"
                    :show-content-header="true"/>
    <main :class="[this.$style.homeMain, !$store.state.device.isNativeApp && $style.desktopWeb]">
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-two-thirds
              nhsuk-u-padding-top-3
               nhsuk-u-padding-bottom-6">
            <connection-error />
            <api-error />
            <flash-message />
            <nuxt />
          </div>
        </div>
      </div>
    </main>
    <div v-if="shouldShowFooter"
         :class="$style['footer-container-desktop']">
      <web-footer/>
    </div>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import NativeVersionSetup from '../services/nativeVersionSetup';
import { findByName } from '@/lib/routes';
import ContentHeader from '@/components/widgets/ContentHeader';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';


export default {
  components: {
    ApiError,
    ConnectionError,
    FlashMessage,
    ContentHeader,
    WebHeader,
    WebFooter,
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
  computed: {
    currentHelpUrl() {
      return findByName(this.$route.name).helpUrl;
    },
    shouldShowFooter() {
      return (
        !this.$store.state.device.isNativeApp
      );
    },
  },
  mounted() {
    NativeVersionSetup(this.$store);
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
    this.configureWebContext(this.currentHelpUrl);
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

<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
</style>

<style module lang="scss" scoped>
@import "../style/home";

.homeMain {
  &.desktopWeb {
    padding: 0;
  }
}


</style>
