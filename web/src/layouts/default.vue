<template>
  <div id="app" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="shouldShowFullDesktopHeader" :class="$style['header-container-desktop']">
      <web-header ref="headerMenu"/>
    </div>
    <div v-else-if="shouldShowSlimDesktopHeader" :class="$style['header-container-desktop']">
      <web-header :show-menu="false" :show-links="false"/>
    </div>
    <div :class="[mainClass, $style['main-container-desktop']]">
      <main :class="mainClass">
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
</template>

<script>
/* eslint-disable no-underscore-dangle */
import Sources from '@/lib/sources';
import A11yTitleAnnouncer from '@/components/widgets/A11yTitleAnnouncer';
import NativeCallbacks from '@/services/native-app';
import HeaderMenu from '@/components/HeaderMenu';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import NavigationMenu from '@/components/NavigationMenu';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SurveyBar from '@/components/SurveyBar';
import HotJar from '@/components/widgets/HotJar';
import { INDEX, LOGIN } from '@/lib/routes';
import NativeVersionSetup from '../services/nativeVersionSetup';

export default {
  components: {
    A11yTitleAnnouncer,
    NavigationMenu,
    HeaderMenu,
    WebHeader,
    WebFooter,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    SurveyBar,
    HotJar,
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
          { innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};URL='/account/signout'">`, body: false },
        ];
      }
    }

    const analyticsScript = [
      {
        src: this.$env.ANALYTICS_SCRIPT_URL,
      },
    ];

    if (this.isAnalyticsCookieAccepted()) {
      head.script = analyticsScript;
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
        this.resetFocus();
      }
    }
  },
  updated() {
    if (this.pathChanged) {
      this.resetFocus();
      this.pathChanged = false;
    }
  },
  methods: {
    isLoginPage() {
      return this.$route.name === LOGIN.name;
    },
    setSurveyBarStatus(isBarOpen) {
      this.surveyBarOpen = isBarOpen;
    },
    isHotJarSurveyVisible() {
      return this.isAnalyticsCookieAccepted() && (this.$env.HOTJAR_SURVEY_VISIBLE === 'true' || this.$env.HOTJAR_SURVEY_VISIBLE === true);
    },
    resetFocus() {
      if (!this.loggedIn) {
        return;
      }
      if (this.resetTimeoutId) {
        clearTimeout(this.resetTimeoutId);
      }
      this.resetTimeoutId = setTimeout(() => {
        if (this.$store.state.device.isNativeApp) {
          NativeCallbacks.resetPageFocus();
        } else {
          const headerMenuCompt = this.$refs.headerMenu;
          if (headerMenuCompt) {
            headerMenuCompt.resetFocusToNhsLogo();
          }
        }
      }, 500);
    },
    isAnalyticsCookieAccepted() {
      return this.$store.state.termsAndConditions.analyticsCookieAccepted;
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
