<!------
__          __     _____  _   _ _____ _   _  _____
\ \        / /\   |  __ \| \ | |_   _| \ | |/ ____|
 \ \  /\  / /  \  | |__) |  \| | | | |  \| | |  __
  \ \/  \/ / /\ \ |  _  /| . ` | | | | . ` | | |_ |
   \  /\  / ____ \| | \ \| |\  |_| |_| |\  | |__| |
    \/  \/_/    \_\_|  \_\_| \_|_____|_| \_|\_____|

THIS LAYOUT FILE IS NO LONGER BEING USED AND WILL SOON BE DELETED.
PLEASE INSTEAD USE nhsuk-layout.vue
!-->
<template>
  <div>
    <modal/>
    <div id="app" ref="nhsAppRoot" :tabindex="!$store.state.device.isNativeApp ? -1 : false"
         :class="{
           [$style.desktopWeb]: !$store.state.device.isNativeApp,
           [$style['nhs-app']]: true
         }">

      <div :class="$style['header-container-desktop']">
        <div v-if="shouldShowFullDesktopHeader">
          <web-header ref="headerMenu"/>
        </div>
        <div v-else-if="shouldShowSlimDesktopHeader">
          <web-header :show-menu="false" :show-links="false"/>
        </div>
        <content-header v-if="!isGpFinderPage()" id="content-header"
                        :show-bread-crumb="shouldShowBreadCrumb"
                        :show-content-header="!isLoginPage()"/>
      </div>

      <div id="maincontent"
           ref="mainContent"
           :tabindex="!$store.state.device.isNativeApp ? -1 : false"
           :class="[mainClass, $style['main-container-desktop']]">
        <main :class="mainClass">
          <header-companion-button v-if="shouldShowButton"/>
          <spinner />
          <connection-error />
          <api-error />
          <flash-message />
          <nuxt/>
        </main>
      </div>

      <survey-bar v-if="showSurvey" :initial-bar-status-open="surveyBarOpen"
                  @onBarStatusChanged="setSurveyBarStatus"/>

      <hot-jar v-if="isAnalyticsCookieAccepted()"/>

      <div v-if="!$store.state.device.isNativeApp" :class="$style['footer-container-desktop']">
        <web-footer/>
      </div>
    </div>
  </div>

</template>

<script>
/* eslint-disable no-underscore-dangle */
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import ContentHeader from '@/components/widgets/ContentHeader';
import FlashMessage from '@/components/widgets/FlashMessage';
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import HotJar from '@/components/widgets/HotJar';
import Modal from '@/components/modal/Modal';
import NativeVersionSetup from '../services/nativeVersionSetup';
import Spinner from '@/components/widgets/Spinner';
import SurveyBar from '@/components/SurveyBar';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import {
  findByName,
  GP_FINDER,
  INDEX,
  isAnonymous,
  LOGIN,
} from '@/lib/routes';

export default {
  components: {
    ApiError,
    ConnectionError,
    ContentHeader,
    FlashMessage,
    HeaderCompanionButton,
    HotJar,
    Modal,
    Spinner,
    SurveyBar,
    WebFooter,
    WebHeader,
  },
  head() {
    let platform = this.$store.state.device.source;
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
          { innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};URL='/account/signout'">`, body: false },
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
    currentHelpUrl() {
      return (this.currentRoute || INDEX).helpUrl;
    },
    currentCrumb() {
      return (this.currentRoute || INDEX).crumb;
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
        this.currentCrumb.nativeDisabled;
    },
    shouldShowFullDesktopHeader() {
      return (
        !this.$store.state.device.isNativeApp &&
        this.shouldShowBreadCrumb
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
      if (this.isLoginPage()) {
        return this.$style.homeMain;
      }
      const clazzes = ['content', 'pull-body'];
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

    this.$store.dispatch('spinner/prevent', false);
  },
  mounted() {
    EventBus.$on(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);

    NativeVersionSetup(this.$store);
    this.configureWebContext(this.currentHelpUrl);
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
  },
  updated() {
    if (this.pathChanged) {
      this.focusNhsAppRoot();
      this.pathChanged = false;
      this.$store.dispatch('spinner/prevent', false);
    }
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);
  },
  methods: {
    isLoginPage() {
      return this.$route.name === LOGIN.name;
    },
    isGpFinderPage() {
      return this.$route.name === GP_FINDER.name;
    },
    setSurveyBarStatus(isBarOpen) {
      this.surveyBarOpen = isBarOpen;
    },
    isHotJarSurveyVisible() {
      return this.isAnalyticsCookieAccepted() && `${this.$env.HOTJAR_SURVEY_VISIBLE}` === 'true';
    },
    isAnalyticsCookieAccepted() {
      return this.$store.state.termsAndConditions.analyticsCookieAccepted;
    },
    focusNhsAppRoot() {
      this.$refs.nhsAppRoot.focus();
    },
  },
};
</script>

<style lang="scss">
  @import "../style/main";
  @import "../style/pulltorefresh";
  @import "../style/elements";
  @import "~nhsuk-frontend/packages/nhsuk";
</style>

<style module lang="scss" scoped>
@import "../style/home";
@import "../style/webshared";

/* Addressing webkit chrome yellow border around app */
.nhs-app:focus {
  outline: none;
}

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

div:focus {
  outline: none !important;
}
</style>
