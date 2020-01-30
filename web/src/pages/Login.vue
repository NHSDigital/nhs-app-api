<template>
  <div id="maincontent" :class="dynamicStyle('loginMain')">
    <div v-if="!isThrottlingEnabled || (isPracticeParticipating
      && isThrottlingEnabled)">
      <h2 class="nhsuk-u-margin-bottom-2">{{ $t('login.desc') }}</h2>
      <form ref="loginForm"
            :action="authoriseUrl"
            method="get">
        <input :value="redirectTo" type="hidden" :name="redirectName">
        <generic-button
          id="login-button"
          :button-classes="['nhsuk-login', 'nhsuk-body', 'nhsuk-button',
                            $store.state.device.isNativeApp
                              ?'button':'']"
          type="submit"
          data-id="login-button"
          @click="trackLogin">
          {{ $t('loginButton.login') }}
        </generic-button>
      </form>
    </div>
    <no-ssr placeholder="">
      <div :class="$style.throttlingContent">
        <p :class="$store.state.device.isNativeApp? $style['center'] : ''">
          <analytics-tracked-tag
            v-if="!hasCookie && isPracticeParticipating && isThrottlingEnabled"
            :class="[!$store.state.device.isNativeApp && $style.desktopWeb]"
            :text="$t('login.checkWhatFeaturesYouCanUse')"
            :click-func="resetAndGoToGPFinder"
            tag="a">
            {{ $t('login.checkWhatFeaturesYouCanUse') }}
          </analytics-tracked-tag>
        </p>
        <div v-if="!isPracticeParticipating && isThrottlingEnabled">
          <nhs-arrow-banner id="btn_organDonation"
                            :banner-text="$t('shared.organDonation.recordDecision')"
                            :click-action="organDonationUrl"
                            :is-analytics-tracked="true"
                            :class="$store.state.device.isNativeApp?
                              'nhsuk-u-margin-bottom-0' : '' "/>
          <div v-if="!isPracticeParticipating && isThrottlingEnabled"
               :class="$store.state.device.isNativeApp?
                 'nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0' : ''">
            <h2 :class="$store.state.device.isNativeApp?
                  'nhsuk-u-margin-bottom-0 nhsuk-heading-xxs' : 'nhsuk-heading-s'"
                style="text-align: left;">
              {{ $t('login.moreFeaturesComingSoon') }}</h2>
            <h3 :class="[$store.state.device.isNativeApp?
              'nhsuk-u-margin-bottom-0' : '', 'nhsuk-heading-xxs']" >
              {{ notParticipatingSurgeryName }}</h3>
            <p :class="$store.state.device.isNativeApp?
              'nhsuk-u-margin-top-0' : ''">{{ notParticipatingSurgeryAddress }}</p>
            <p :class="$store.state.device.isNativeApp? $style['center'] : ''">
              <analytics-tracked-tag :text="$t('login.notMyGpSurgery')"
                                     :class="[$style.checkFeaturesLink,
                                              !$store.state.device.isNativeApp
                                                && $style.desktopWeb]"
                                     :click-func="resetAndGoToGPFinder"
                                     href="#"
                                     tag="a">
                {{ $t('login.notMyGpSurgery') }}
              </analytics-tracked-tag>
            </p>
          </div>
        </div>
      </div></no-ssr>
    <div v-if="this.$store.state.device.isNativeApp"
         :class="[$style['nhsuk-body-s'], $style['appVersion']]">
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </div>
  </div>
</template>
<script>
import moment from 'moment';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import AuthorisationService from '@/services/authorisation-service';
import GenericButton from '@/components/widgets/GenericButton';
import NativeCallbacks from '@/services/native-app';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { setCookie } from '@/lib/cookie-manager';
import { BEGINLOGIN, REDIRECT_PARAMETER } from '@/lib/routes';

export default {
  layout: 'login',
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    NhsArrowBanner,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN.path,
      hasCookie: false,
      isButtonDisabled: false,
      organDonationUrl: this.$store.app.$env.ORGAN_DONATION_THROTTLING_URL,
      practiceAddress: undefined,
      practiceName: undefined,
      practiceParticipating: true,
      redirectTo: this.$route.query[REDIRECT_PARAMETER],
      redirectName: REDIRECT_PARAMETER,
    };
  },
  computed: {
    isPracticeParticipating() {
      return this.practiceParticipating;
    },
    isThrottlingEnabled() {
      return (this.$store.app.$env.THROTTLING_ENABLED === 'true' ||
            this.$store.app.$env.THROTTLING_ENABLED === true);
    },
    notParticipatingSurgeryName() {
      return this.practiceName;
    },
    notParticipatingSurgeryAddress() {
      return this.practiceAddress;
    },
    shouldShowBiometrics() {
      return this.$store.app.$env.BIOMETRICS_ENABLED && this.$store.state.device.isNativeApp;
    },
  },
  asyncData(context) {
    const { store, app } = context;
    const betaCookie = store.$cookies.get('BetaCookie');
    if (betaCookie && !betaCookie.PracticeParticipating && betaCookie.ODSCode
        && !betaCookie.Skipped) {
      return app.$http.getV1Odscodelookup({
        odsCode: betaCookie.ODSCode,
      }).then((response) => {
        const practiceParticipating = response && response.isGpSystemSupported;
        betaCookie.PracticeParticipating = practiceParticipating;

        setCookie({
          cookies: store.$cookies,
          key: 'BetaCookie',
          value: betaCookie,
          options: {
            maxAge: moment.duration(1, 'y').asSeconds(),
            secure: store.app.$env.SECURE_COOKIES,
          },
        });

        return {
          practiceParticipating,
          practiceName: betaCookie.PracticeName,
          practiceAddress: betaCookie.PracticeAddress,
          hasCookie: true,
        };
      }).catch(() => Promise.resolve());
    }
    return {
      practiceParticipating: true,
      practiceName: undefined,
      practiceAddress: undefined,
      hasCookie: false,
    };
  },
  mounted() {
    sessionStorage.removeItem('hasAgreedToMedicalWarning');
    if (this.shouldShowBiometrics && this.$route.query.fidoAuthResponse) {
      const { authoriseUrl, loginUrl } = this.generateRedirectData();
      this.authoriseUrl = authoriseUrl;
      this.$store.app.context.redirect(loginUrl);
      this.$store.dispatch('analytics/satelliteTrack', 'fidoLogin');
      return;
    }
    const betaCookie = this.$cookies.get('BetaCookie');
    const throttlingEnabled = this.$store.app.$env.THROTTLING_ENABLED === true || this.$store.app.$env.THROTTLING_ENABLED === 'true';

    if (throttlingEnabled && !betaCookie) {
      this.$store.dispatch('device/goToGPFinder');
      this.isButtonDisabled = true;
      return;
    }

    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideElements();
      if (throttlingEnabled && betaCookie && betaCookie.Skipped) {
        NativeCallbacks.storeBetaCookie();
      }
    }
  },
  methods: {
    generateRedirectData() {
      const authorisationService = new AuthorisationService(this.$store.app.$env);
      const { request, loginUrl } = authorisationService.generateLoginUrl({
        isNativeApp: this.$store.state.device.isNativeApp,
        cookies: this.$cookies,
        redirectTo: this.redirectTo,
        fidoAuthResponse: this.$route.query.fidoAuthResponse,
      });
      return { authoriseUrl: request.authoriseUrl, loginUrl };
    },
    trackLogin() {
      if (!this.isButtonDisabled) {
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        this.isButtonDisabled = true;
      }
    },
    resetAndGoToGPFinder() {
      setCookie({
        cookies: this.$store.app.$cookies,
        key: 'BetaCookie',
        value: {},
        options: {
          maxAge: -1,
          secure: this.$store.app.$env.SECURE_COOKIES,
        },
      });
      this.$store.dispatch('device/goToGPFinder');
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/accessibility";
@import '../style/colours';
@import "../style/home";
@import "../style/listmenu";
.no-decoration {
  text-decoration: none;
}

.list-menu {
  margin-left: -1.3em;
  margin-right: -1.3em;
  margin-top: -1em;
  li {
    a {
      h3 {
        text-align: left;
        padding-top: 0;
        color: $nhs_blue;
      }
    }
  }
}
.center {
  text-align: center;
}
.appVersion {
  text-align: center;
}
.throttlingContent+.appVersion {
  margin-top: 0.5em;
}
</style>
