<template>
  <div id="app"
       ref="nhsAppRoot"
       tabindex="-1">
    <div>
      <web-header v-if="showWebHeader"
                  :show-menu="false"
                  :show-links="false"
                  :show-header-buttons="false"/>
    </div>
    <content-header id="content-header"
                    :show-bread-crumb="false"
                    :show-content-header="true"
                    class="nhsuk-u-margin-bottom-3"/>
    <main id="maincontent" ref="mainContent" tabindex="-1">
      <spinner/>
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-two-thirds
              nhsuk-u-padding-top-3
               nhsuk-u-padding-bottom-6">
            <connection-error />
            <api-error />
            <flash-message />
            <slot />
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
import get from 'lodash/fp/get';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import ContentHeader from '@/components/widgets/ContentHeader';
import FlashMessage from '@/components/widgets/FlashMessage';
import NativeApp from '@/services/native-app';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinnerMixin';
import Spinner from '@/components/widgets/Spinner';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  components: {
    ApiError,
    ConnectionError,
    ContentHeader,
    FlashMessage,
    Spinner,
    WebFooter,
    WebHeader,
  },
  mixins: [ResetSpinnerMixin, OnUpdateTitleMixin],
  metaInfo() {
    const head = {
      htmlAttrs: { lang: this.$t('language') },
      title: `${this.title} - ${this.$t('appTitle')}`,
      // TODO: needed if 'nojs' is not a thing?
      __dangerouslyDisableSanitizers: ['noscript'],
    };

    // TODO: needed if 'nojs' is not a thing?
    const durationSeconds = get('durationSeconds', this.$store.$cookies.get('nhso.session'));

    if (durationSeconds) {
      head.noscript = [{
        innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};URL='/account/signout'">`,
        body: false,
      }];
    }

    const { analyticsCookieAccepted } = this.$store.state.termsAndConditions;
    const analyticsScriptUrl = this.$store.$env.ANALYTICS_SCRIPT_URL;

    if (analyticsScriptUrl !== 'NOT_SET' && analyticsCookieAccepted) {
      head.script = [{
        src: analyticsScriptUrl,
      }];
    }

    return head;
  },
  data() {
    return {
      header: '',
      title: '',
    };
  },
  beforeRouteUpdate(to, _, next) {
    EventBus.$emit(UPDATE_HEADER, to.meta);
    this.onUpdateTitle(to.meta);
    next();
  },
  computed: {
    currentHelpUrl() {
      return this.$route.meta.helpUrl;
    },
    shouldShowFooter() {
      return !this.$store.state.device.isNativeApp;
    },
    showWebHeader() {
      return NativeApp.shouldShowPreLoginHeader();
    },
  },
  created() {
    if (process.browser) {
      this.$store.dispatch('session/updateLastCalledAt');
    }

    const appVersion = this.$store.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
  },
  mounted() {
    EventBus.$emit(UPDATE_HEADER, this.$route.meta);
    EventBus.$emit(UPDATE_TITLE, this.$route.meta);
    NativeVersionSetup(this.$store);
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
    this.configureWebContext(this.currentHelpUrl);
    if (this.$store.state.device.isNativeApp) {
      NativeApp.dismissProgressBar();
    }
  },
  methods: {
    focusNhsAppRoot() {
      this.$refs.nhsAppRoot.focus();
    },
  },
};
</script>

<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
</style>
