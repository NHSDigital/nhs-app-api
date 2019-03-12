<template>
  <div :class="dynamicStyle('loginMain')">
    <div v-if="isPracticeParticipating">
      <h2>{{ $t('login.desc') }}</h2>
      <form :action="authoriseUrl" method="get" @submit="formSubmitted">
        <input :value="scope" type="hidden" name="scope">
        <input v-model="clientId" type="hidden" name="client_id">
        <input :value="codeChallenge" type="hidden" name="code_challenge">
        <input :value="codeChallengeMethod" type="hidden" name="code_challenge_method">
        <input :value="redirectUri" type="hidden" name="redirect_uri">
        <input :value="state" type="hidden" name="state">
        <input :value="responseType" type="hidden" name="response_type">
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
import Sources from '@/lib/sources';
import { setCookie } from '@/lib/cookie-manager';
import { BEGINLOGIN, GP_FINDER } from '@/lib/routes';
import AuthorisationService from '@/services/authorisation-service';
import NativeCallbacks from '@/services/native-app';
import moment from 'moment';
import querystring from 'querystring';
import { getDynamicStyle } from '@/lib/desktop-experience';

export default {
  layout: 'login',
  components: {
    AnalyticsTrackedTag,
    LoginButton,
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN.path,
      scope: '',
      codeChallenge: '',
      codeChallengeMethod: '',
      redirectUri: '',
      state: '',
      responseType: '',
      clientId: '',
      fidoAuthResponse: '',
      practiceParticipating: true,
      practiceName: undefined,
      practiceAddress: undefined,
      isButtonDisabled: false,
      source: this.getSource(),
    };
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
  computed: {
    isPracticeParticipating() {
      return this.practiceParticipating;
    },
    shouldShowBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && (this.$store.state.device.source === 'android' || this.$store.state.device.source === 'ios');
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
  mounted() {
    const betaCookie = this.$cookies.get('BetaCookie');
    const throttlingEnabled = this.$env.THROTTLING_ENABLED === true || this.$env.THROTTLING_ENABLED === 'true';

    if (throttlingEnabled && !betaCookie && this.$store.state.device.isNativeApp) {
      this.goToUrl(GP_FINDER.path);
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

    const authorisationService = new AuthorisationService(this.$env);
    const loginValues = authorisationService.generateLoginValues(
      this.$route.query.source,
      this.$cookies,
      this.$route.query.fidoAuthResponse,
    );

    this.scope = loginValues.scope;
    this.codeChallenge = loginValues.codeChallenge;
    this.codeChallengeMethod = loginValues.codeChallengeMethod;
    this.redirectUri = loginValues.redirectUri;
    this.state = loginValues.state;
    this.responseType = loginValues.responseType;
    this.clientId = loginValues.clientId;
    this.authoriseUrl = loginValues.authoriseUrl;

    this.fidoAuthResponse = loginValues.fidoAuthResponse;

    if (this.shouldShowBiometrics && this.fidoAuthResponse) {
      this.$store.app.context.redirect(this.generateFidoUrl());
    }
  },
  methods: {
    async formSubmitted() {
      if (process.client) {
        if (this.$store.state.device.source === Sources.Android) {
          // (Android only)
          // Disable login button on click.
          // Page should be refreshed onResume.
          this.isButtonDisabled = true;
        }
        this.$store.dispatch('analytics/satelliteTrack', 'login');
      }
      return true;
    },
    generateFidoUrl() {
      const originalData = this.$data;
      const newData = {};
      Object.keys(originalData).forEach((key) => {
        if (key !== 'authoriseUrl') {
          newData[this.camelToUnderscore(key)] = originalData[key];
        }
      });
      return `${this.authoriseUrl}?${querystring.stringify(newData)}`;
    },
    camelToUnderscore(key) {
      return key.replace(/([A-Z])/g, '_$1').toLowerCase();
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
    getSource() {
      return this.$route.query.source;
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
