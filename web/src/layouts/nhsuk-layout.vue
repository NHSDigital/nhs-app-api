<template>
  <div>
    <modal/>
    <div id="app" ref="nhsAppRoot" :tabindex="!$store.state.device.isNativeApp ? -1 : false">
      <div v-if="shouldShowFullDesktopHeader">
        <web-header ref="headerMenu"/>
      </div>
      <div v-else-if="shouldShowSlimDesktopHeader">
        <web-header :show-menu="false" :show-links="false"/>
      </div>
      <content-header id="content-header"
                      :show-bread-crumb="shouldShowBreadCrumb"
                      :show-content-header="!isLoginPage()"/>

      <div id="maincontent" ref="mainContent" tabindex="-1">
        <main :class="mainClass">
          <spinner/>
          <div class="nhsuk-width-container">
            <div class="nhsuk-grid-row">
              <div class="nhsuk-grid-column-two-thirds
              nhsuk-u-padding-top-3
               nhsuk-u-padding-bottom-6">
                <connection-error :with-title="true"/>
                <api-error :with-title="true"/>
                <flash-message/>
                <nuxt/>
              </div>
            </div>
          </div>
        </main>
      </div>

      <survey-bar v-if="showSurvey" :initial-bar-status-open="surveyBarOpen"
                  @onBarStatusChanged="setSurveyBarStatus"/>

      <hot-jar v-if="isAnalyticsCookieAccepted()"/>

      <div v-if="!$store.state.device.isNativeApp">
        <web-footer/>
      </div>
    </div>
  </div>

</template>

<script>
/* eslint-disable no-underscore-dangle */
import {
  getCrumbTrailForRoute,
  findByName,
  INDEX,
  LOGIN,
} from '@/lib/routes';
import ContentHeader from '@/components/widgets/ContentHeader';
import NativeCallbacks from '@/services/native-app';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SurveyBar from '@/components/SurveyBar';
import HotJar from '@/components/widgets/HotJar';
import NativeVersionSetup from '../services/nativeVersionSetup';
import Modal from '@/components/modal/Modal';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

export default {
  components: {
    ContentHeader,
    WebHeader,
    WebFooter,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    SurveyBar,
    HotJar,
    Modal,
  },
  head() {
    let { platform } = this.$store.state.device.source;
    const { nativeVersion } = this.$store.state.appVersion;

    if (nativeVersion !== undefined) {
      platform = `${platform} (${nativeVersion})`;
    }

    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
      meta: [
        { name: 'web version', content: this.$store.state.appVersion.webVersion },
        { name: 'platform', content: platform },
      ],
      __dangerouslyDisableSanitizers: ['noscript'],
    };

    const sessionCookie = this.$store.app.$cookies.get('nhso.session');

    if (sessionCookie) {
      const { durationSeconds } = sessionCookie;

      if (durationSeconds) {
        head.noscript = [
          {
            innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};url='/login?showExpiryMessage=true'">`,
            body: false,
          },
        ];
      }
    }

    if (this.$env.ANALYTICS_SCRIPT_URL !== 'NOT_SET' && this.isAnalyticsCookieAccepted()) {
      const analyticsScript = [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ];

      if (this.isAnalyticsCookieAccepted()) {
        head.script = analyticsScript;
      }
    }
    return head;
  },
  data() {
    return {
      surveyBarOpen: true,
      pathChanged: false,
      resetTimeoutId: undefined,
    };
  },
  computed: {
    currentBreadCrumbs() {
      return getCrumbTrailForRoute(findByName(this.$route.name));
    },
    currentHelpUrl() {
      return findByName(this.$route.name).helpUrl;
    },
    showMenu() {
      return (
        !this.$store.state.device.isNativeApp &&
          this.loggedIn &&
          this.$route.name !== 'Login'
      );
    },
    shouldShowButton() {
      return (
        !this.$store.getters['errors/showApiError']
          && !this.$store.state.device.isNativeApp
      );
    },
    shouldShowBreadCrumb() {
      return (
        this.loggedIn &&
        this.$route.name !== 'Login'
      );
    },
    shouldShowFullDesktopHeader() {
      return (
        !this.$store.state.device.isNativeApp &&
          this.loggedIn &&
          this.$route.name !== 'Login'
      );
    },
    shouldShowSlimDesktopHeader() {
      return (
        !this.$store.state.device.isNativeApp &&
          !this.loggedIn
      );
    },
    showSurvey() {
      return (this.isHotJarSurveyVisible() && this.$route.name === INDEX.name);
    },
    mainClass() {
      const clazzes = [];
      if (this.$store.state.device.isNativeApp) {
        clazzes.push('native');
        clazzes.push('web');
      } else {
        clazzes.push('desktopWeb');
      }
      if (this.isHotJarSurveyVisible() && this.$route.name === INDEX.name) {
        if (this.surveyBarOpen) {
          clazzes.push('survey-open');
        } else {
          clazzes.push('survey-closed');
        }
      }
      return clazzes;
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
  },

  watch: {
    $route(to, from) {
      if (from !== to) {
        this.pathChanged = true;
        this.setHelpUrl(this.currentHelpUrl);
      }
    },
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
  mounted() {
    EventBus.$on(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);

    NativeVersionSetup(this.$store, this.$route);
    if (this.loggedIn) {
      this.$store.dispatch('session/startValidationChecking');
      window.validateSession =
          window.validateSession || (() => {
            this.$store.dispatch('session/validate');
          });

      if (this.$store.state.device.isNativeApp) {
        this.$store.dispatch('auth/nativeLogin');
      }
      if (this.$store.state.device.isNativeApp) {
        NativeCallbacks.resetPageFocus();
      }
    }
    this.setHelpUrl(this.currentHelpUrl);
  },
  updated() {
    if (this.pathChanged) {
      this.focusNhsAppRoot();
      this.pathChanged = false;
    }
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);
  },
  methods: {
    isAnalyticsCookieAccepted() {
      return this.$store.state.termsAndConditions.analyticsCookieAccepted;
    },
    isHotJarSurveyVisible() {
      return this.isAnalyticsCookieAccepted() && `${this.$env.HOTJAR_SURVEY_VISIBLE}` === 'true';
    },
    isLoginPage() {
      return this.$route.name === LOGIN.name;
    },
    focusNhsAppRoot() {
      this.$refs.nhsAppRoot.focus();
    },
    setSurveyBarStatus(isBarOpen) {
      this.surveyBarOpen = isBarOpen;
    },
  },
};
</script>


<style lang="scss" scoped>
  @import "~nhsuk-frontend/packages/nhsuk";
</style>
