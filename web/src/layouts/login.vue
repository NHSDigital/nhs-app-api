<template>
  <div>
    <div v-if="hasConnectionProblem" :class="$style['error-container']">
      <connection-error/>
      <api-error/>
    </div>
    <div v-else id="app" :class="{ [$style['no-footer']]: isNativeApp }">
      <div :class="dynamicStyle('login-app-header-flex-container')">
        <home-header v-if="isNativeApp"/>
        <div v-else>
          <web-header :show-menu="false" :show-links="false" :show-header-buttons="false"/>
        </div>
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
                {{ $t('login.loginToYourNhsAccountToAccessServices') }}</p>
              <p class="nhsuk-u-margin-bottom-4">{{ $t('login.loginToYourAccountTo') }}</p>
              <ul>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.getYourCovidPass') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.orderRepeatPrescriptions') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.bookAndManageAppointments') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.getHealthInfoAndAdvice') }}</li>
                <li class="nhsuk-u-margin-bottom-3">{{ $t('login.viewYourHealthRecord') }}</li>
                <li>{{ $t('login.viewYourNhsNumber') }}</li>
              </ul>
            </div>
            <div class="nhsuk-grid-column-one-third"
                 :class="$style['nhs-app-mobile-hide']">
              <div class="nhsuk-card nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0">
                <download-app-panel id="desktop-app-panel"/>
              </div>
            </div>
          </div>
          <pre-registration-information :should-show-full-content="true"/>
          <main :class="[$style['homeMain-desktop'], $style['pull-content']]">
            <flash-message/>
            <slot/>
            <div class="nhsuk-grid-row">
              <div class="nhsuk-grid-column-one-third"
                   :class="$style['nhs-app-desktop-hide']">
                <div class="nhsuk-card nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-4">
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
import FlashMessage from '@/components/widgets/FlashMessage';
import HomeHeader from '@/components/HomeHeader';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import WebFooter from '@/components/widgets/WebFooter';
import WebHeader from '@/components/widgets/WebHeader';
import DownloadAppPanel from '@/components/widgets/DownloadAppPanel';
import { GET_HEALTH_ADVICE_PATH } from '@/router/paths';
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
    FlashMessage,
    HomeHeader,
    WebFooter,
    WebHeader,
    DownloadAppPanel,
  },
  metaInfo() {
    return {
      titleTemplate: (titleChunk) => {
        const titleSuffix = this.isNativeApp ? this.$t('appTitle') : this.$t('appTitleOnline');
        return `${titleChunk} - ${titleSuffix}`;
      },
      title: this.$t('navigation.pages.titles.login'),
      htmlAttrs: {
        lang: this.$t('language'),
      },
      meta: [
        { name: 'description', content: this.$t('login.metaDescriptionAccessYourServices') },
      ],
    };
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      symptomButtonId: 'btn_home_symptoms',
      symptomsUrl: GET_HEALTH_ADVICE_PATH,
    };
  },
  mounted() {
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
  @import "../style/custom/login-layout";

</style>
