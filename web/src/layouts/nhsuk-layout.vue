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
      <content-header v-if="!isGpFinderPage" id="content-header"
                      :show-bread-crumb="shouldShowBreadCrumb"
                      :show-content-header="shouldShowContentHeader"/>
      <div id="maincontent" ref="mainContent" tabindex="-1">
        <main :class="mainClass">
          <spinner/>
          <div class="nhsuk-width-container">
            <div :class="getRowClass">
              <div :class="[getColumnClass,
                            'nhsuk-u-padding-top-3', 'nhsuk-u-padding-bottom-6']">
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
import { get } from 'lodash/fp';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import ContentHeader from '@/components/widgets/ContentHeader';
import FlashMessage from '@/components/widgets/FlashMessage';
import HotJar from '@/components/widgets/HotJar';
import Modal from '@/components/modal/Modal';
import NativeCallbacks from '@/services/native-app';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import Spinner from '@/components/widgets/Spinner';
import SurveyBar from '@/components/SurveyBar';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import {
  findByName,
  getCrumbTrailForRoute,
  GP_FINDER,
  INDEX,
  isAnonymous,
  LOGIN,
  DOCUMENT_DETAIL,
} from '@/lib/routes';

export default {
  components: {
    ApiError,
    ConnectionError,
    ContentHeader,
    FlashMessage,
    HotJar,
    Modal,
    Spinner,
    SurveyBar,
    WebHeader,
    WebFooter,
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
            innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};URL='/account/signout'">`,
            body: false,
          },
        ];
      }
    }

    if (this.$env.ANALYTICS_SCRIPT_URL !== 'NOT_SET' && this.isAnalyticsCookieAccepted() && !isAnonymous(this.$route.name)) {
      head.script = [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ];
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
    currentRoute() {
      return findByName(this.$route.name);
    },
    currentBreadCrumbs() {
      return getCrumbTrailForRoute(this.currentRoute);
    },
    currentHelpUrl() {
      return (this.currentRoute || INDEX).helpUrl;
    },
    currentCrumb() {
      return (this.currentRoute || INDEX).crumb;
    },
    isGpFinderPage() {
      return this.$route.name === GP_FINDER.name;
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
    // document needs to stretch to make use of
    // more of the screen
    getColumnClass() {
      return this.$route.name === DOCUMENT_DETAIL.name &&
        !this.$store.state.device.isNativeApp ?
        'nhsuk-grid-column-full-width' : 'nhsuk-grid-column-two-thirds';
    },
    getRowClass() {
      return this.$route.name === DOCUMENT_DETAIL.name &&
        !this.$store.state.device.isNativeApp ?
        '' : 'nhsuk-grid-row';
    },
    showMenu() {
      return (
        !this.$store.state.device.isNativeApp &&
          this.loggedIn &&
          this.$route.name !== LOGIN.name
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
        this.$route.name !== LOGIN.name &&
        !this.breadcrumbDisabledNative
      );
    },
    breadcrumbDisabledNative() {
      return this.$store.state.device.isNativeApp &&
        get('nativeDisabled')(this.currentCrumb);
    },
    shouldShowContentHeader() {
      // the shouldShowContentHeader field is only
      // defined if we do not need to show it
      const route = findByName(this.$route.name);
      return this.loggedIn && route.shouldShowContentHeader === undefined;
    },
    shouldShowFullDesktopHeader() {
      return (
        !this.$store.state.device.isNativeApp &&
          this.loggedIn &&
          this.$route.name !== LOGIN.name
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
  },

  watch: {
    $route(to, from) {
      if (from !== to) {
        this.pathChanged = true;
        this.configureWebContext(this.currentHelpUrl);
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
        NativeCallbacks.resetPageFocus();
      }
    }
    this.configureWebContext(this.currentHelpUrl);
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
    focusNhsAppRoot() {
      this.$refs.nhsAppRoot.focus();
    },
    setSurveyBarStatus(isBarOpen) {
      this.surveyBarOpen = isBarOpen;
    },
  },
};
</script>

<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
</style>
