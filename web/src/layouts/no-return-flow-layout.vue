<template>
  <div id="app"
       ref="nhsAppRoot"
       tabindex="-1">
    <div>
      <web-header :show-menu="false"
                  :show-links="false"
                  :show-header-buttons="false"
                  :logo-link="false"/>
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
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import ContentHeader from '@/components/widgets/ContentHeader';
import FlashMessage from '@/components/widgets/FlashMessage';
import NativeCallbacks from '@/services/native-app';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinnerMixin';
import Spinner from '@/components/widgets/Spinner';
import WebHeader from '@/components/widgets/WebHeader';
import { FOCUS_NHSAPP_ROOT, UPDATE_HEADER, EventBus } from '@/services/event-bus';

export default {
  components: {
    ApiError,
    ConnectionError,
    ContentHeader,
    FlashMessage,
    Spinner,
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
  },
  created() {
    const appVersion = this.$store.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
  },
  beforeMount() {
    EventBus.$on(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);
  },
  mounted() {
    EventBus.$emit(UPDATE_HEADER, this.$route.meta);
    NativeVersionSetup(this.$store);
    this.configureWebContext(this.currentHelpUrl);
    NativeCallbacks.dismissProgressBar();
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);
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
