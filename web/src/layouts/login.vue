<template>
  <div>
    <div v-if="isErrorVisible" :class="$style['error-container']">
      <connection-error/>
      <api-error/>
    </div>
    <div v-else id="app" :class="$style['login-app-header']">
      <div :class="dynamicStyle('login-app-header-flex-container')">
        <home-header v-if="this.$store.state.device.isNativeApp"/>
        <div v-else :class="$style['header-container-desktop']">
          <web-header :show-menu="false" :show-links="false"/>
        </div>
        <session-expired-banner v-if="showSessionExpiredBanner"/>
        <div v-if="this.$store.state.device.isNativeApp">
          <main :class="this.$style.homeMain">
            <flash-message/>
            <nuxt/>
          </main>
        </div>
        <div v-else :class="$style['main-container-desktop']">
          <div :class="$style['banner-container']">
            <div>
              <beta-banner :banner-class="[$style.banner]" data-sid="beta-flag"/>
            </div>
          </div>
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
import HomeHeader from '@/components/HomeHeader';
import Spinner from '@/components/widgets/Spinner';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';
import NativeCallbacks from '@/services/native-app';
import { getDynamicStyle } from '@/lib/desktop-experience';
import WebHeader from '@/components/widgets/WebHeader';
import WebFooter from '@/components/widgets/WebFooter';
import SymptomBanner from '@/components/SymptonBanner';
import BetaBanner from '@/components/BetaBanner';
import NativeVersionSetup from '../services/nativeVersionSetup';
import CookieBanner from '../components/CookieBanner';

export default {
  components: {
    CookieBanner,
    BetaBanner,
    SymptomBanner,
    WebHeader,
    WebFooter,
    Spinner,
    ApiError,
    ConnectionError,
    FlashMessage,
    HomeHeader,
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
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} screen`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
          async: true,
        },
      ],

    };
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
    if (Sources.isNative(this.$route.query.source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
      this.$store.dispatch('device/setSourceDevice', this.$route.query.source);
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
.banner-container {
  background: $white;
  padding-top: 1em;
  padding-bottom:  0.7em;
}

.banner-container > div {
  margin: 0 16px;
  max-width: 960px;
}

@media (min-width: 48.0625em) {
  .banner-container > div {
    margin: 0 32px;
    padding: 0 16px;
  }
}

@media (min-width: 1024px) {
  .banner-container > div {
    margin: 0 auto;
    padding: 0 16px;
  }
}

.login-app-header {
  position: absolute;
  left:0;
  top:0;
  right:0;
  bottom:0;
}

.login-app-header-flex-container {
  display:flex;
  flex-direction:column;
  height:100%;
}

.login-app-header-flex-container-desktop {
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  justify-content: flex-start;
  align-content: stretch;
  align-items: flex-start;
  position: absolute;
  margin: 0 auto;
  top: 0;
  right: 0;
  left: 0;
  bottom: 0;
  background: #f0f4f5;
  width: 100%;
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
  max-width: 960px;
  display: block;
  margin: 0 auto;
  padding: 0 16px 2.5em;
}


@media (min-width: 48.0625em) {
  section {
    margin: 0 32px;
  }
}

@media (min-width: 1024px) {
  section  {
    margin: 0 auto;
  }
}

</style>
