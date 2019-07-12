<template>
  <div>
    <div v-if="isErrorVisible" :class="$style['error-container']">
      <connection-error/>
      <api-error/>
    </div>
    <div v-else id="app">
      <div :class="dynamicStyle('login-app-header-flex-container')">
        <home-header v-if="this.$store.state.device.isNativeApp"/>
        <div v-else :class="$style['header-container-desktop']">
          <web-header :show-menu="false" :show-links="false" :show-header-buttons="false"/>
        </div>
        <session-expired-banner v-if="showSessionExpiredBanner"/>
        <div v-if="this.$store.state.device.isNativeApp">
          <main :class="this.$style.homeMain">
            <flash-message/>
            <nuxt/>
          </main>
        </div>
        <div v-else id="mainContent" ref="mainContent" tabindex="-1"
             :class="$style['main-container-desktop']">
          <section :class="$style['pull-content']">
            <h1 :class="$style['web-page-title']">{{ $t('web.home.title') }}</h1>
            <ul :class="$style['intro-bullets']">
              <li>{{ $t('web.home.bullets.one') }}</li>
              <li>{{ $t('web.home.bullets.two') }}</li>
              <li>{{ $t('web.home.bullets.three') }}</li>
              <li>{{ $t('web.home.bullets.four') }}</li>
            </ul>

            <main :class="[this.$style['homeMain-desktop'], this.$style['pull-content']]">
              <flash-message/>
              <nuxt/>
              <symptom-banner/>
            </main>
          </section>
        </div>
        <div v-if="!this.$store.state.device.isNativeApp"
             :class="$style['footer-container-desktop']">
          <web-footer/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import Sources from '@/lib/sources';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';
import HomeHeader from '@/components/HomeHeader';
import NativeCallbacks from '@/services/native-app';
import { getDynamicStyle } from '@/lib/desktop-experience';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import SymptomBanner from '@/components/SymptomBanner';
import NativeVersionSetup from '../services/nativeVersionSetup';

export default {
  components: {
    HomeHeader,
    SymptomBanner,
    WebHeader,
    WebFooter,
    ApiError,
    ConnectionError,
    FlashMessage,
    SessionExpiredBanner,
  },
  computed: {
    showSessionExpiredBanner() {
      return this.$store.state.session.showExpiryMessage || this.$route.query.showExpiryMessage;
    },
    isErrorVisible() {
      return this.$store.getters['errors/showApiError'] || this.$store.state.errors.hasConnectionProblem;
    },
  },
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} screen`,
      script: [],
    };
    if (this.$env.ANALYTICS_SCRIPT_URL !== 'NOT_SET') {
      head.script = [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
          async: true,
        },
      ];
    }
    return head;
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      this.$nextTick(() => {
        NativeCallbacks.attemptBiometricLogin();
      });
    }
    NativeVersionSetup(this.$store, this.$route);
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
  },
  created() {
    const { source } = this.$route.query;

    if (source) {
      this.$store.dispatch('device/updateIsNativeApp', Sources.isNative(source));
      this.$store.dispatch('device/setSourceDevice', source);
    }

    const appVersion = this.$store.app.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
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
@import "../style/spacings";
@import "../style/webshared";

.error-container {
  @include space(padding, all, 1em);
}

.login-app-header-flex-container {
  display:flex;
  flex-direction:column;
  height:100vh;
}

.login-app-header-flex-container-desktop {
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  justify-content: flex-start;
  align-content: stretch;
  align-items: flex-start;
  margin: 0 auto;
  background: #f0f4f5;
  width: auto;
  min-height: 100vh;
}

.error-container {
  @include space(padding, all, 1em);
}

.intro-bullets {
  margin: 0 2em;
  line-height: 250%;
}

.appVersion {
  text-align: center;
  color: #637683;
  font-size: small;
}

.sub-header {
  margin: 0.5em 0;
}

.rule {
  height: 0.063em;
  border: none;
  background-color: #D8DDE0;
}

.nhsuk-icon__arrow-right-circle-desktop {
  display: inline-block;
  vertical-align: middle;
  fill: #007f3b;
  height: 1.2em;
  left: -3px;
  top: -6px;
  width: 1.2em;
}

section {
  @include main-container-width;
  display: block;
  margin: 0 auto;
  padding: 0 16px 2.5em;
}


@include fromTablet {
  section {
    margin: 0 32px;
  }
}

@include fromDesktop {
  section  {
    margin: 0 auto;
  }
}

</style>
