<template>
  <div>
    <modal/>
    <div id="app" ref="nhsAppRoot" :tabindex="!$store.state.device.isNativeApp ? -1 : false"
         :class="{
           [$style.desktopWeb]: !$store.state.device.isNativeApp,
           [$style['nhs-app']]: true
         }">
      <div v-if="shouldShowFullDesktopHeader" :class="$style['header-container-desktop']">
        <web-header ref="headerMenu"/>
      </div>
      <div v-else-if="shouldShowSlimDesktopHeader" :class="$style['header-container-desktop']">
        <web-header :show-menu="false" :show-links="false"/>
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
import Sources from '@/lib/sources';
import NativeCallbacks from '@/services/native-app';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import SurveyBar from '@/components/SurveyBar';
import HotJar from '@/components/widgets/HotJar';
import { INDEX, LOGIN } from '@/lib/routes';
import NativeVersionSetup from '../services/nativeVersionSetup';
import Modal from '@/components/modal/Modal';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

export default {
  components: {
    WebHeader,
    WebFooter,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    HeaderCompanionButton,
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
          { innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};url='/login?showExpiryMessage=true'">`, body: false },
        ];
      }
    }

    if (this.$env.ANALYTICS_SCRIPT_URL !== 'NOT_SET') {
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
    $route(from, to) {
      if (from !== to) {
        this.pathChanged = true;
      }
    },
  },
  created() {
    if (Sources.isNative(this.$route.query.source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
    } else {
      this.$store.dispatch('device/updateIsNativeApp', false);
    }
    this.$store.dispatch('device/setSourceDevice', this.$route.query.source);

    if (process.browser) {
      this.$store.dispatch('session/updateLastCalledAt');
    }

    const appVersion = this.$store.app.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
  },
  mounted() {
    if (process.client) {
      EventBus.$on(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);

      this.$store.subscribe((mutation) => {
        if (mutation.type === 'myRecord/ACCEPT_TERMS') {
          this.focusNhsAppRoot();
        }
      });

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
      }
    }
  },
  updated() {
    if (this.pathChanged) {
      this.focusNhsAppRoot();
      this.pathChanged = false;
      NativeCallbacks.pageLoadComplete();
    }
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_NHSAPP_ROOT, this.focusNhsAppRoot);
  },
  methods: {
    isLoginPage() {
      return this.$route.name === LOGIN.name;
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
