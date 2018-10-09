<template>
  <div id="app">
    <header-menu v-if="showMenu"/>
    <main :class="mainClass">
      <spinner />
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt/>
    </main>
    <survey-bar v-if="showSurvey" :initial-bar-status-open="surveyBarOpen"
                @onBarStatusChanged="setSurveyBarStatus"/>
    <navigation-menu v-if="showMenu"/>
    <hot-jar />
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import HeaderMenu from '@/components/HeaderMenu';
import NavigationMenu from '@/components/NavigationMenu';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SurveyBar from '@/components/SurveyBar';
import HotJar from '@/components/widgets/HotJar';
import { INDEX, LOGIN } from '@/lib/routes';
import Sources from '@/lib/sources';

export default {
  components: {
    NavigationMenu,
    HeaderMenu,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    SurveyBar,
    HotJar,
  },
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };
    return head;
  },
  data() {
    return {
      surveyBarOpen: true,
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
  },
  mounted() {
    if (process.client) {
      if (this.loggedIn) {
        this.$store.dispatch('session/startValidationChecking');
        window.validateSession =
          window.validateSession || (() => {
            this.$store.dispatch('session/validate');
          }
          );

        this.$store.dispatch('auth/nativeLogin');
      }
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
      return this.$env.HOTJAR_SURVEY_VISIBLE === 'true' || this.$env.HOTJAR_SURVEY_VISIBLE === true;
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
</style>
