<template>
  <div>
    <div v-if="isErrorVisible" :class="$style['error-container']">
      <connection-error/>
      <api-error/>
    </div>
    <div v-else id="app">
      <corona-virus-banner v-if="this.$store.state.device.isNativeApp"/>
      <div :class="dynamicStyle('login-app-header-flex-container')">
        <home-header v-if="this.$store.state.device.isNativeApp"/>
        <div v-else>
          <web-header :show-menu="false" :show-links="false" :show-header-buttons="false"/>
          <corona-virus-banner/>
        </div>
        <session-expired-banner v-if="showSessionExpiredBanner"/>
        <div v-if="this.$store.state.device.isNativeApp">
          <main :class="this.$style.homeMain">
            <flash-message/>
            <nuxt/>
          </main>
        </div>
        <div v-else id="mainContent" ref="mainContent" tabindex="-1"
             class="nhsuk-width-container">
          <div class="nhsuk-grid-row">
            <div class="nhsuk-grid-column-full">
              <h1 id="title" class="nhsuk-u-margin-top-4">{{ $t('web.home.title') }}</h1>
            </div>
          </div>
          <div class="nhsuk-grid-row">
            <div class="nhsuk-grid-column-two-thirds nhsuk-u-margin-bottom-0">
              <p class="nhsuk-u-margin-bottom-4">{{ $t('web.home.bulletListDescription') }}</p>
              <ul>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.one') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.two') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('web.home.bullets.three') }}</li>
                <li>{{ $t('web.home.bullets.four') }}</li>
              </ul>
            </div>
            <div class="nhsuk-grid-column-one-third"
                 :class="$style['nhs-app-mobile-hide']">
              <div class="nhsuk-panel nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0">
                <download-app-panel id="desktop-app-panel"/>
              </div>
            </div>
          </div>
          <div class="nhsuk-grid-row">
            <div class="nhsuk-grid-column-two-thirds">
              <div id="before-you-start" class="nhsuk-u-margin-top-1">
                <h2 class="nhsuk-u-margin-bottom-3">
                  {{ $t('web.home.beforeYouStartTitle') }}</h2>
                <p>{{ $t('web.home.beforeYouStartBulletListDescription') }}</p>
                <ul>
                  <li class="nhsuk-u-margin-bottom-3">
                    {{ $t('web.home.beforeYouStartBullets.one') }}
                  </li>
                  <li class="nhsuk-u-margin-bottom-3">
                    {{ $t('web.home.beforeYouStartBullets.two') }}
                  </li>
                </ul>
                <details id="age-info" class="nhsuk-details">
                  <summary class="nhsuk-details__summary">
                    <span class="nhsuk-details__summary-text">
                      {{ $t('web.home.aged13To15InformationTitle') }}
                    </span>
                  </summary>
                  <div class="nhsuk-details__text">
                    <p>{{ $t('web.home.aged13To15Description') }}</p>
                  </div>
                </details>
              </div>
            </div>
          </div>
          <main :class="[this.$style['homeMain-desktop'], this.$style['pull-content']]">
            <flash-message/>
            <nuxt/>
            <div class="nhsuk-grid-row">
              <div class="nhsuk-grid-column-one-third"
                   :class="$style['nhs-app-desktop-hide']">
                <div class="nhsuk-panel nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-4">
                  <download-app-panel id="mobile-app-panel"/>
                </div>
              </div>
            </div>
            <div id="other-services">
              <p>{{ $t('web.home.otherServicesTitle') }}</p>
              <ul>
                <li class="nhsuk-u-margin-bottom-3">
                  <a href="https://111.nhs.uk/service/COVID-19/" target="_blank" rel="noopener noreferrer">
                    {{ $t('web.home.otherServicesBullets.one') }}
                  </a>
                </li>
                <li class="nhsuk-u-margin-bottom-3">
                  <a href="https://www.nhs.uk/conditions/" target="_blank" rel="noopener noreferrer">
                    {{ $t('web.home.otherServicesBullets.two') }}
                  </a>
                </li>
                <li class="nhsuk-u-margin-bottom-3">
                  <a href="https://111.nhs.uk/" target="_blank" rel="noopener noreferrer">
                    {{ $t('web.home.otherServicesBullets.three') }}
                  </a>
                </li>
              </ul>
            </div>
          </main>
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
import SessionExpiredBanner from '@/components/SessionExpiredBanner';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import DownloadAppPanel from '@/components/widgets/DownloadAppPanel';
import { CHECKYOURSYMPTOMS, findByName } from '@/lib/routes';
import { getDynamicStyle } from '@/lib/desktop-experience';


export default {
  layout: 'nhsuk-layout',
  components: {
    ApiError,
    ConnectionError,
    CoronaVirusBanner,
    FlashMessage,
    HomeHeader,
    SessionExpiredBanner,
    WebFooter,
    WebHeader,
    DownloadAppPanel,
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
  @import '../style/screensizes';
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
    display: flex;
    flex-direction: column;
    height: 90vh;
  }

  @include desktopAndBelow(){
    .nhs-app-mobile-hide {
      display: none;
    }
  }

  @include fromDesktop(){
    .nhs-app-desktop-hide {
      display: none;
    }
  }

  .login-app-header-full-container {
    height: 100%;
  }

  a {
    display: inline-block;
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

</style>
