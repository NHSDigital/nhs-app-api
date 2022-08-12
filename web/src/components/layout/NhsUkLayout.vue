<template>
  <div>
    <modal/>
    <div id="app"
         ref="nhsAppRoot"
         :tabindex="!$store.state.device.isNativeApp ? -1 : false">
      <slot name="header">
        <div v-if="shouldShowFullDesktopHeader">
          <web-header ref="headerMenu"/>
        </div>
        <div v-else-if="shouldShowSlimDesktopHeader">
          <web-header :show-menu="false" :show-links="false"/>
        </div>

        <content-header id="content-header"
                        :show-bread-crumb="shouldShowBreadCrumb"
                        :show-content-header="shouldShowContentHeader"/>

        <welcome-banner v-if="isIndexRoute" id="welcome-banner" />
      </slot>

      <div id="maincontent" ref="mainContent" tabindex="-1">
        <main :class="mainClass">
          <spinner/>
          <div class="nhsuk-width-container">
            <div :class="getRowClass">
              <div :class="[getColumnClass,
                            'nhsuk-u-padding-top-3', 'nhsuk-u-padding-bottom-6']">
                <connection-error :with-title="true"/>
                <api-error :with-title="true" aria-live="off"/>
                <flash-message/>
                <slot/>
              </div>
            </div>
          </div>
        </main>
      </div>

      <slot name="footer">
        <survey-bar v-if="showSurvey" :initial-bar-status-open="surveyBarOpen"
                    @onBarStatusChanged="setSurveyBarStatus"/>

        <hot-jar v-if="isAnalyticsCookieAccepted()"/>
        <qualtrics-intercept v-if="isQualtricsVisible()"/>

        <div v-if="!$store.state.device.isNativeApp">
          <web-footer/>
        </div>
      </slot>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import ApiError from '@/components/errors/ApiError';
import ClearBackLinkOverrideMixin from '@/plugins/mixinDefinitions/ClearBackLinkOverrideMixin';
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import ConnectionError from '@/components/errors/ConnectionError';
import ContentHeader from '@/components/widgets/ContentHeader';
import FlashMessage from '@/components/widgets/FlashMessage';
import HotJar from '@/components/widgets/HotJar';
import isFunction from 'lodash/fp/isFunction';
import Modal from '@/components/modal/Modal';
import NativeApp from '@/services/native-app';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import QualtricsIntercept from '@/components/widgets/QualtricsIntercept';
import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinnerMixin';
import showShutterPage from '@/lib/proxy/shutter';
import Spinner from '@/components/widgets/Spinner';
import SurveyBar from '@/components/SurveyBar';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import WelcomeBanner from '@/components/WelcomeBanner';
import { INDEX_CRUMB } from '@/breadcrumbs/general';
import {
  DOCUMENT_DETAIL_NAME,
  LOGIN_NAME,
  INDEX_NAME,
} from '@/router/names';

export default {
  name: 'NhsUkLayout',
  components: {
    ApiError,
    ConnectionError,
    ContentHeader,
    FlashMessage,
    HotJar,
    Modal,
    QualtricsIntercept,
    Spinner,
    SurveyBar,
    WebFooter,
    WebHeader,
    WelcomeBanner,
  },
  mixins: [
    ClearBackLinkOverrideMixin,
    ResetSpinnerMixin,
    CalculateAgeInMonthsAndYears,
  ],
  props: {
    hasFooter: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      resetTimeoutId: undefined,
      surveyBarOpen: true,
    };
  },
  computed: {
    currentCrumb() {
      return get('$route.meta.crumb', this) || INDEX_CRUMB;
    },
    noFooter() {
      return !this.hasFooter || this.$store.state.device.isNativeApp;
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
    // document needs to stretch to make use of
    // more of the screen
    getColumnClass() {
      return this.$route.name === DOCUMENT_DETAIL_NAME &&
        !this.$store.state.device.isNativeApp ?
        'nhsuk-grid-column-full-width' : 'nhsuk-grid-column-two-thirds';
    },
    getRowClass() {
      return this.$route.name === DOCUMENT_DETAIL_NAME &&
        !this.$store.state.device.isNativeApp ?
        '' : 'nhsuk-grid-row';
    },
    showMenu() {
      return !this.$store.state.device.isNativeApp &&
          this.loggedIn &&
          this.$route.name !== LOGIN_NAME;
    },
    shouldShowButton() {
      return !this.$store.getters['errors/showApiError'] && !this.$store.state.device.isNativeApp;
    },
    shouldShowBreadCrumb() {
      return this.loggedIn &&
        this.$route.name !== LOGIN_NAME &&
        !this.breadcrumbDisabledNative;
    },
    breadcrumbDisabledNative() {
      const { previousQuestion, demographicsQuestionAnswered, carePlans } =
      this.$store.state.onlineConsultations;
      return (this.$store.state.device.isNativeApp && this.nativeDisabled)
        || (previousQuestion === undefined && demographicsQuestionAnswered)
        || carePlans !== undefined;
    },
    nativeDisabled() {
      return isFunction(this.currentCrumb.nativeDisabled)
        ? this.currentCrumb.nativeDisabled(this)
        : this.currentCrumb.nativeDisabled;
    },
    shouldShowContentHeader() {
      return get('meta.shouldShowContentHeader', this.$route) !== false;
    },
    shouldShowFullDesktopHeader() {
      return !this.$store.state.device.isNativeApp &&
        this.loggedIn &&
        this.$route.name !== LOGIN_NAME;
    },
    shouldShowSlimDesktopHeader() {
      return !this.$store.state.device.isNativeApp && !this.loggedIn;
    },
    showSurvey() {
      return this.isHotJarSurveyVisible() && this.$route.name === INDEX_NAME;
    },
    mainClass() {
      const clazzes = [];
      if (this.$store.state.device.isNativeApp) {
        clazzes.push('native');
        clazzes.push('web');
      } else {
        clazzes.push('desktopWeb');
      }
      if (this.isHotJarSurveyVisible() && this.$route.name === INDEX_NAME) {
        if (this.surveyBarOpen) {
          clazzes.push('survey-open');
        } else {
          clazzes.push('survey-closed');
        }
      }
      return clazzes;
    },
    isLoading() {
      return this.$store.getters['http/isLoading'];
    },
    isIndexRoute() {
      return this.$route.name === INDEX_NAME;
    },
  },
  watch: {
    $route(to, from) {
      if (from !== to) {
        this.configureWebContext(this.currentHelpUrl);
      }
    },
    isLoading(to, from) {
      if (from && !to) {
        showShutterPage(this.$route, this);
      }
    },
  },
  created() {
    this.$store.dispatch('biometrics/showBiometricSpinner', false);

    this.$store.dispatch('session/updateLastCalledAt');

    NativeVersionSetup(this.$store);

    this.$store.dispatch('loginSettings/fetchBiometricStatus');

    if (this.loggedIn) {
      this.$store.dispatch('session/startValidationChecking');
      window.validateSession =
        window.validateSession || (() => {
          this.$store.dispatch('session/validate');
        });

      if (this.$store.state.device.isNativeApp) {
        this.$store.dispatch('auth/nativeLogin');
      }
    }
    this.configureWebContext(this.currentHelpUrl);

    const appVersion = this.$store.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeApp.dismissProgressBar();
    }
  },
  methods: {
    isAnalyticsCookieAccepted() {
      return this.$store.state.termsAndConditions.analyticsCookieAccepted;
    },
    isQualtricsVisible() {
      return this.isAnalyticsCookieAccepted() && this.$store.$env.QUALTRICS_SURVEY_ENABLED;
    },
    isHotJarSurveyVisible() {
      return this.isAnalyticsCookieAccepted() && this.$store.$env.HOTJAR_SURVEY_VISIBLE;
    },
    setSurveyBarStatus(isBarOpen) {
      this.surveyBarOpen = isBarOpen;
    },
  },
};
</script>
<style lang="scss" module scoped>
  @import "@/style/custom/home-banner";
</style>
