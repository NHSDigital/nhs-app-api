<template>
  <div>
    <div v-if="isErrorVisible" :class="$style['error-container']">
      <connection-error/>
      <api-error/>
    </div>
    <div v-else id="app">
      <corona-virus-banner v-if="this.$store.state.device.isNativeApp" />
      <div :class="dynamicStyle('login-app-header-flex-container')">
        <home-header v-if="this.$store.state.device.isNativeApp"/>
        <div v-else :class="$style['header-container-desktop']">
          <web-header :show-menu="false" :show-links="false" :show-header-buttons="false"/>
          <corona-virus-banner />
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
          <section>
            <h1 class="nhsuk-u-margin-top-4">{{ $t('web.home.title') }}</h1>
            <ul>
              <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.one') }}</li>
              <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.two') }}</li>
              <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.three') }}</li>
              <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.four') }}</li>
            </ul>

            <main :class="[this.$style['homeMain-desktop'], this.$style['pull-content']]">
              <flash-message/>
              <nuxt/>
              <h3 class="nhsuk-u-margin-bottom-2">{{ $t('symptomBanner.howAreYouFeeling') }}</h3>
              <nhs-arrow-banner :id="symptomButtonId"
                                :banner-text="$t('symptomBanner.checker')"
                                :click-action="symptomsUrl"
                                :is-analytics-tracked="false"
                                :open-new-window="false"/>
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
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import CoronaVirusBanner from '@/components/widgets/CoronaVirusBanner';
import FlashMessage from '@/components/widgets/FlashMessage';
import HomeHeader from '@/components/HomeHeader';
import NativeCallbacks from '@/services/native-app';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import SessionExpiredBanner from '@/components/SessionExpiredBanner';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import { CHECKYOURSYMPTOMS, findByName } from '@/lib/routes';
import { getDynamicStyle } from '@/lib/desktop-experience';

export default {
  components: {
    ApiError,
    ConnectionError,
    CoronaVirusBanner,
    FlashMessage,
    HomeHeader,
    NhsArrowBanner,
    SessionExpiredBanner,
    WebFooter,
    WebHeader,
  },
  data() {
    return {
      currentHelpUrl: findByName(this.$route.name).helpUrl,
      symptomButtonId: 'btn_home_symptoms',
      symptomsUrl: CHECKYOURSYMPTOMS.path,
    };
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
    };
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      this.$nextTick(() => {
        NativeCallbacks.attemptBiometricLogin();
      });
    }
    NativeVersionSetup(this.$store);
    window.validateSession =
      window.validateSession || (() => this.$store.dispatch('session/validate'));
    this.configureWebContext(this.currentHelpUrl);
  },
  created() {
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
@import "~nhsuk-frontend/packages/nhsuk";
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
  height:90vh;
}

.login-app-header-full-container {
  height: 100%;
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

.appVersion {
  text-align: center;
}

.sub-header {
  margin: 0.5em 0;
}

.rule {
  height: 0.063em;
  border: none;
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
