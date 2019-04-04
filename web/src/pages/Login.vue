<template>
  <div :class="dynamicStyle('loginMain')">
    <div v-if="isPracticeParticipating">
      <h2>{{ $t('login.desc') }}</h2>
      <form ref="loginForm"
            :action="authoriseUrl"
            method="get"
            @submit.prevent="redirectToCitizenId">
        <input :value="source" type="hidden" name="source">
        <LoginButton :disabled="isButtonDisabled" />
      </form>
    </div>
    <no-ssr placeholder="">
      <div :class="$style.throttlingContent">
        <analytics-tracked-tag
          v-if="showCheckFeaturesLink && isPracticeParticipating"
          :class="[$style.checkFeaturesLink, !$store.state.device.isNativeApp && $style.desktopWeb]"
          :text="$t('login.checkWhatFeaturesYouCanUse')"
          :click-func="resetAndGoToGPFinder"
          tag="a"
          tabindex="0">
          {{ $t('login.checkWhatFeaturesYouCanUse') }}
        </analytics-tracked-tag>
        <div v-if="!isPracticeParticipating && this.$store.state.device.isNativeApp">
          <h2 :class="$style.moreFeaturesComingSoon">{{ $t('login.moreFeaturesComingSoon') }}</h2>
          <h5>{{ notParticipatingSurgeryName }}</h5>
          <p>{{ notParticipatingSurgeryAddress }}</p>
          <analytics-tracked-tag :text="$t('login.notMyGpSurgery')" :class="$style.notMySurgeryLink"
                                 :click-func="resetAndGoToGPFinder" tag="a">
            {{ $t('login.notMyGpSurgery') }}
          </analytics-tracked-tag>
        </div>
      </div>
    </no-ssr>
    <div v-if="this.$store.state.device.isNativeApp" :class="$style.appVersion">
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </div>
  </div>
</template>
<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import LoginButton from '@/components/LoginButton';
import GenericButton from '@/components/widgets/GenericButton';
import { setCookie } from '@/lib/cookie-manager';
import { BEGINLOGIN, GP_FINDER } from '@/lib/routes';
import AuthorisationService from '@/services/authorisation-service';
import NativeCallbacks from '@/services/native-app';
import moment from 'moment';
import { getDynamicStyle } from '@/lib/desktop-experience';

export default {
  layout: 'login',
  components: {
    AnalyticsTrackedTag,
    LoginButton,
    GenericButton,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN.path,
      source: this.getSource(),
      practiceParticipating: true,
      practiceName: undefined,
      practiceAddress: undefined,
      isButtonDisabled: false,
    };
  },
  computed: {
    isPracticeParticipating() {
      return this.practiceParticipating;
    },
    shouldShowBiometrics() {
      return this.$store.app.$env.BIOMETRICS_ENABLED && this.$store.state.device.isNativeApp;
    },
    notParticipatingSurgeryName() {
      return this.practiceName;
    },
    notParticipatingSurgeryAddress() {
      return this.practiceAddress;
    },
    showCheckFeaturesLink() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');
      return betaCookie !== undefined && !betaCookie.NeverShowCheckFeatureLink &&
          (this.$store.app.$env.THROTTLING_ENABLED === 'true' ||
            this.$store.app.$env.THROTTLING_ENABLED === true);
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
        };
      }).catch(() => Promise.resolve());
    }
    return {
      practiceParticipating: true,
      practiceName: undefined,
      practiceAddress: undefined,
    };
  },
  mounted() {
    if (this.shouldShowBiometrics && this.$route.query.fidoAuthResponse) {
      const { authoriseUrl, loginUrl } = this.generateRedirectData();
      this.authoriseUrl = authoriseUrl;
      this.$store.app.context.redirect(loginUrl);
      this.$store.dispatch('analytics/satelliteTrack', 'fidoLogin');
      return;
    }
    const betaCookie = this.$cookies.get('BetaCookie');
    const throttlingEnabled = this.$store.app.$env.THROTTLING_ENABLED === true || this.$store.app.$env.THROTTLING_ENABLED === 'true';

    if (throttlingEnabled && !betaCookie && this.$store.state.device.isNativeApp) {
      this.goToUrl(GP_FINDER.path);
      this.isButtonDisabled = true;
      return;
    }

    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
      NativeCallbacks.hideMenuBar();
      NativeCallbacks.hideHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
      if (throttlingEnabled && betaCookie && betaCookie.Skipped) {
        NativeCallbacks.storeBetaCookie();
      }
    }
  },
  methods: {
    generateRedirectData() {
      const authorisationService = new AuthorisationService(this.$store.app.$env);
      const { source } = this.$route.query;
      const { request, loginUrl } = authorisationService.generateLoginUrl({
        source,
        cookies: this.$cookies,
        fidoAuthResponse: this.$route.query.fidoAuthResponse,
      });
      return { authoriseUrl: request.authoriseUrl, loginUrl };
    },
    redirectToCitizenId() {
      if (!this.isButtonDisabled) {
        const { authoriseUrl, loginUrl } = this.generateRedirectData();
        this.authoriseUrl = authoriseUrl;
        this.$store.app.context.redirect(loginUrl);
        this.isButtonDisabled = true;
        this.$store.dispatch('analytics/satelliteTrack', 'login');
      }
    },
    getSource() {
      return this.$route.query.source;
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
      this.goToUrl(GP_FINDER.path);
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import "../style/home";
 .appVersion {
  text-align: center;
  color: #637683;
  font-size: small;
 }
 .throttlingContent+.appVersion {
  margin-top: 0.5em;
 }
</style>
