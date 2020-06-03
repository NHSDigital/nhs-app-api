<template>
  <div id="app" :class="{ [$style['no-footer']]: !shouldShowFooter }">
    <div>
      <web-header :show-menu="false"
                  :show-links="false"
                  :show-header-buttons="false"/>
    </div>
    <content-header id="content-header"
                    :show-bread-crumb="false"
                    :show-content-header="true"
                    class="nhsuk-u-margin-bottom-3"/>
    <main id="maincontent" ref="mainContent">
      <spinner/>
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
    <div v-if="shouldShowFooter">
      <web-footer/>
    </div>
  </div>
</template>

<script>
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import ContentHeader from '@/components/widgets/ContentHeader';
import FlashMessage from '@/components/widgets/FlashMessage';
import NativeCallbacks from '@/services/native-app';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinner';
import Spinner from '@/components/widgets/Spinner';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import { findByName } from '@/lib/routes';


export default {
  components: {
    ApiError,
    ConnectionError,
    ContentHeader,
    FlashMessage,
    Spinner,
    WebHeader,
    WebFooter,
  },
  mixins: [ResetSpinnerMixin],
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
      return !this.$store.state.device.isNativeApp;
    },
  },
  mounted() {
    NativeVersionSetup(this.$store);
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
    this.configureWebContext(this.currentHelpUrl);
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.dismissProgressBar();
    }
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
  @import "~nhsuk-frontend/packages/nhsuk";
</style>

<style module lang="scss" scoped>
  @import "../style/nofooter";
</style>
