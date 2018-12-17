<template>
  <div :class="$style.loginMain">
    <div v-if="isPracticeParticipating">
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
      <a v-if="showCheckFeaturesLink" :class="$style.checkFeaturesLink"
         :href="gpFinderResetLink">
        {{ $t('login.checkWhatFeaturesYouCanUse') }}
      </a>
      <div :class="$style.appVersion">
        Version {{ this.$store.state.appVersion.webVersion }}
        <span v-if="this.$store.state.appVersion.nativeVersion">
          ({{ this.$store.state.appVersion.nativeVersion }})
        </span>
      </div>
    </div>
    <div>
      <div v-if="!isPracticeParticipating">
        <h2 :class="$style.moreFeaturesComingSoon">{{ $t('login.moreFeaturesComingSoon') }}</h2>
        <h5>{{ notParticipatingSurgeryName }}</h5>
        <p>{{ notParticipatingSurgeryAddress }}</p>
        <a :class="$style.notMySurgeryLink" :href="gpFinderResetLink">
          {{ $t('login.notMyGpSurgery') }}
        </a>
      </div>
    </div>
  </div>
</template>
<script>
import AuthorisationService from '@/services/authorisation-service';
import LoginButton from '@/components/LoginButton';

import { BEGINLOGIN, GP_FINDER } from '@/lib/routes';
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
      isPracticeParticipating: true,
      practiceName: undefined,
      practiceAddress: undefined,
    };
  },
  asyncData(context) {
    const { store, app } = context;
    const betaCookie = store.$cookies.get('BetaCookie');
    if (betaCookie && !betaCookie.PracticeParticipating && betaCookie.ODSCode) {
      return app.$http.getV1Odscodelookup({
        odsCode: betaCookie.ODSCode,
      }).then((response) => {
        const isPracticeParticipating = response && response.isGpSystemSupported;
        betaCookie.PracticeParticipating = isPracticeParticipating;

        store.$cookies.set('BetaCookie', betaCookie, {
          path: '/',
          maxAge: moment.duration(1, 'y').asSeconds(),
          secure: this.$env.SECURE_COOKIES,
        });

        return {
          isPracticeParticipating,
          practiceName: betaCookie.PracticeName,
          practiceAddress: betaCookie.PracticeAddress,
        };
      }).catch(() => Promise.resolve());
    }
    return {};
  },
  computed: {
    shouldShowBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.$store.state.device.source === 'android';
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
    gpFinderResetLink() {
      return `${GP_FINDER.path}?reset=true`;
    },
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
      NativeCallbacks.hideMenuBar();
      NativeCallbacks.hideHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
      const betaCookie = this.$cookies.get('BetaCookie');
      if ((this.$env.THROTTLING_ENABLED === true || this.$env.THROTTLING_ENABLED === 'true') &&
          betaCookie && betaCookie.Skipped) {
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
