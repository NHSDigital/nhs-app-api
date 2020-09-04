<template>
  <div>
    <div v-if="isErrorVisible" :class="$style['error-container']">
      <connection-error/>
      <api-error/>
    </div>
    <div v-else id="app" :class="{ [$style['no-footer']]: isNativeApp }">
      <corona-virus-banner v-if="isNativeApp"
                           :should-be-floating="true"/>
      <div :class="dynamicStyle('login-app-header-flex-container')">
        <home-header v-if="isNativeApp"/>
        <div v-else>
          <web-header :show-menu="false" :show-links="false" :show-header-buttons="false"/>
          <corona-virus-banner/>
        </div>
        <session-expired-banner v-if="showSessionExpiredBanner"/>
        <main v-if="isNativeApp" :class="$style.homeMain">
          <flash-message/>
          <slot id="mainContent"/>
        </main>
        <div v-else id="maincontent" ref="mainContent" tabindex="-1"
             class="nhsuk-width-container">
          <div class="nhsuk-grid-row">
            <div class="nhsuk-grid-column-full">
              <h1
                id="title"
                class="nhsuk-u-margin-top-4">{{ $t('login.accessYourNhsServices') }}</h1>
            </div>
          </div>
          <div class="nhsuk-grid-row">
            <div class="nhsuk-grid-column-two-thirds nhsuk-u-margin-bottom-0">
              <p v-if="!isNativeApp" id="desktopSpecificInformation">
                {{ $t('login.useNhsAppOnlineToAccessServices') }}</p>
              <p class="nhsuk-u-margin-bottom-4">{{ $t('login.useThisServiceTo') }}</p>
              <ul>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.bookAndManageAppointments') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.orderRepeatPrescriptions') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.checkSymptomsAndGetAdvice') }}</li>
                <li>{{ $t('login.viewYourMedicalRecord') }}</li>
              </ul>
            </div>
            <div class="nhsuk-grid-column-one-third"
                 :class="$style['nhs-app-mobile-hide']">
              <div class="nhsuk-panel nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0">
                <download-app-panel id="desktop-app-panel"/>
              </div>
            </div>
          </div>
          <pre-registration-information :should-show-header="true"/>
          <main :class="[$style['homeMain-desktop'], $style['pull-content']]">
            <flash-message/>
            <slot/>
            <div class="nhsuk-grid-row">
              <div class="nhsuk-grid-column-one-third"
                   :class="$style['nhs-app-desktop-hide']">
                <div class="nhsuk-panel nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-4">
                  <download-app-panel id="mobile-app-panel"/>
                </div>
              </div>
            </div>
            <other-services/>
          </main>
        </div>
        <div v-if="!isNativeApp"
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
import { CHECKYOURSYMPTOMS_PATH } from '@/router/paths';
import { getDynamicStyle } from '@/lib/desktop-experience';
import PreRegistrationInformation from '@/components/PreRegistrationInformation';
import OtherServices from '../components/OtherServices';

export default {
  name: 'LoginLayout',
  components: {
    OtherServices,
    PreRegistrationInformation,
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
  metaInfo() {
    return {
      title: this.$t('navigation.pages.titles.login'),
      htmlAttrs: {
        lang: this.$t('language'),
      },
    };
  },
  data() {
    return {
      currentHelpUrl: this.$route.meta.helpUrl,
      isNativeApp: this.$store.state.device.isNativeApp,
      symptomButtonId: 'btn_home_symptoms',
      symptomsUrl: CHECKYOURSYMPTOMS_PATH,
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
    const appVersion = this.$store.$env.VERSION_TAG;
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
  @import "~nhsuk-frontend/packages/nhsuk";
</style>

<style module lang="scss" scoped>
  @import "../style/home";
  @import "../style/spacings";
  @import "../style/webshared";
  @import "../style/nofooter";

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
