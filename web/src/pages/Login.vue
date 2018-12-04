<template>
  <div :class="$style.loginMain">
    <div v-if="surgeryParticipating">
      <h2>{{ $t('login.desc') }}</h2>
      <form :action="authoriseUrl" method="get">
        <input :value="scope" type="hidden" name="scope">
        <input v-model="clientId" type="hidden" name="client_id">
        <input :value="codeChallenge" type="hidden" name="code_challenge">
        <input :value="codeChallengeMethod" type="hidden" name="code_challenge_method">
        <input :value="redirectUri" type="hidden" name="redirect_uri">
        <input :value="state" type="hidden" name="state">
        <input :value="responseType" type="hidden" name="response_type">
        <LoginButton />
      </form>
      <a v-if="betaCookiePresent" :class="$style.checkFeaturesLink"
         @click="wrongGpSurgery()">{{ $t('login.checkWhatFeaturesYouCanUse') }}</a>
      <div :class="$style.appVersion">
        Version {{ this.$store.state.appVersion.webVersion }}
        <span v-if="this.$store.state.appVersion.nativeVersion">
          ({{ this.$store.state.appVersion.nativeVersion }})
        </span>
      </div>
    </div>
    <div>
      <div v-if="!surgeryParticipating">
        <h2 :class="$style.moreFeaturesComingSoon">{{ $t('login.moreFeaturesComingSoon') }}</h2>
        <h5>{{ notParticipatingSurgeryName }}</h5>
        <p>{{ notParticipatingSurgeryAddress }}</p>
        <a :class="$style.notMySurgeryLink" @click="wrongGpSurgery()">
          {{ $t('login.notMyGpSurgery') }}
        </a>
      </div>
    </div>
  </div>
</template>
<script>
import AuthorisationService from '@/services/authorisation-service';
import LoginButton from '@/components/LoginButton';

import { BEGINLOGIN, LOGIN, GP_FINDER_PARTICIPATION } from '@/lib/routes';
import NativeCallbacks from '@/services/native-app';
import querystring from 'querystring';
import moment from 'moment';

export default {
  head() {
    return {
      title: 'Login Screen',
    };
  },
  layout: 'login',
  components: {
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
    };
  },
  computed: {
    shouldShowBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.$store.state.device.source === 'android';
    },
    surgeryParticipating() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');

      if (betaCookie !== undefined) {
        const practiceParticipating = betaCookie.PracticeParticipating;
        if (practiceParticipating !== undefined && practiceParticipating === false) {
          return false;
        }
      }

      return true;
    },
    notParticipatingSurgeryName() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');
      if (betaCookie !== undefined) {
        return betaCookie.PracticeName;
      }
      return null;
    },
    notParticipatingSurgeryAddress() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');
      if (betaCookie !== undefined) {
        return betaCookie.PracticeAddress;
      }
      return null;
    },
    betaCookiePresent() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');
      return betaCookie !== undefined;
    },
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
      NativeCallbacks.hideMenuBar();
      NativeCallbacks.hideHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
      const betaCookie = this.$cookies.get('BetaCookie');
      if (this.$env.THROTTLING_ENABLED && betaCookie && betaCookie.Skipped) {
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
    wrongGpSurgery() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');
      delete betaCookie.ODSCode;
      delete betaCookie.PracticeParticipating;
      this.$store.app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: moment.duration(1, 'y').asSeconds(),
      });
      this.$router.go(LOGIN.path);
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
    getHrefForGpPractice(odsCode, name, address) {
      return `${GP_FINDER_PARTICIPATION.path}?odsCode=${odsCode}` +
        `&practiceName=${encodeURIComponent(name)}` +
        `&practiceAddress=${encodeURIComponent(address)}` +
        `&source=${this.$store.state.device.source}`;
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
</style>
