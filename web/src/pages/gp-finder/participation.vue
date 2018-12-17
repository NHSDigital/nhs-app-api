<template>
  <div>
    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ headerText }} </h1>
      <form :action="backLink" method="get">
        <input :value="true" type="hidden" name="reset">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <button tabindex="0" type="submit">
          <back-icon/>
        </button>
      </form>
    </header>
    <div v-if="showTemplate" :class="[$style.webHeader, $style.throttlingContent, 'pull-content']">

      <h2>{{ `${$t('th04.featuresHeader')} ${practiceName}` }}</h2>
      <form :action="backLink" method="get">
        <input :value="true" type="hidden" name="reset">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <a :href="`${backLink}?reset=true`" :class="$style.linkBack">
          {{ $t('th04.ctaNotMySurgery') }}</a>
      </form>
      <hr>

      <h2>{{ $t('th04.currentlyAvailableHeader') }}</h2>
      <ul id="availableFeatures" :class="$style.availableFeatures">
        <li v-for="feature in availableFeatures" :key="feature">{{ feature }}</li>
      </ul>

      <div v-if="unavailableFeatures">
        <h2>{{ $t('th04.comingSoonHeader') }}</h2>
        <ul id="unavailableFeatures" :class="$style.unavailableFeatures">
          <li v-for="feature in unavailableFeatures" :key="feature">{{ feature }}</li>
        </ul>
      </div>

      <p v-if="participating" id="createAccountMessage" :class="$style.createAccountMessage">
        {{ $t('th04.createAccountMessage') }}
      </p>

      <form v-if="participating" :action="authoriseUrl" method="get">
        <input :value="scope" type="hidden" name="scope">
        <input :value="clientId" type="hidden" name="client_id">
        <input :value="codeChallenge" type="hidden" name="code_challenge">
        <input :value="codeChallengeMethod" type="hidden" name="code_challenge_method">
        <input :value="redirectUri" type="hidden" name="redirect_uri">
        <input :value="state" type="hidden" name="state">
        <input :value="responseType" type="hidden" name="response_type">
        <generic-button :class="[$style.button, $style.green, $style.continue]" type="submit">
          {{ this.$t('th04.ctaContinue') }}
        </generic-button>
      </form>
      <a v-else :href="sendingEmail" :class="[$style.button, $style.green, $style.continue]">
        {{ this.$t('th04.ctaContinue') }}
      </a>

      <p v-if="participating" id="limitingFeaturesWarning">
        {{ $t('th04.limitingFeaturesWarning') }}
      </p>

    </div>
  </div>
</template>

<script>
import AuthorisationService from '@/services/authorisation-service';
import { INDEX, GP_FINDER, BEGINLOGIN, GP_FINDER_SENDING_EMAIL } from '@/lib/routes';
import BackIcon from '@/components/icons/BackIcon';
import GenericButton from '@/components/widgets/GenericButton';
import moment from 'moment';
import NativeCallbacks from '@/services/native-app';

export default {
  layout: 'throttling',
  components: {
    BackIcon,
    GenericButton,
  },
  beforeRouteEnter(to, from, next) {
    if (to.query.odsCode) {
      return next();
    }
    return next(INDEX.path);
  },
  head() {
    return {
      title: `${this.$t('th04.title')} - ${this.$t('appTitle')}`,
    };
  },
  data() {
    return {
      practiceName: undefined,
      practiceAddress: undefined,
      odsCode: undefined,
      participating: false,
      defaultFeatures: this.$t('th04.defaultFeatures'),
      conditionalFeatures: this.$t('th04.conditionalFeatures'),
      authoriseUrl: BEGINLOGIN.path,
      scope: '',
      codeChallenge: '',
      codeChallengeMethod: '',
      redirectUri: '',
      state: '',
      responseType: '',
      clientId: '',
      headerText: this.$t('th04.header'),
      backLink: GP_FINDER.path,
      sendingEmail: GP_FINDER_SENDING_EMAIL.path,
    };
  },
  asyncData(context) {
    return context.store.app.$http
      .getV1Odscodelookup({ odsCode: context.route.query.odsCode })
      .then((response) => {
        const participating = response && response.isGpSystemSupported;
        let data = { participating };

        if (participating) {
          const authorisationService = new AuthorisationService(context.store.app.$env);
          const loginValues = authorisationService.generateLoginValues(
            context.route.query.source,
            context.store.$cookies,
          );

          data = Object.assign(data, loginValues);
        }
        return data;
      });
  },
  computed: {
    availableFeatures() {
      return this.participating ?
        this.defaultFeatures.concat(this.conditionalFeatures) : this.defaultFeatures;
    },
    unavailableFeatures() {
      return !this.participating ? this.conditionalFeatures : undefined;
    },
    practiceParticipating() {
      return this.participating;
    },
  },
  created() {
    const { query } = this.$route;
    const { router } = this.$store.app;

    if (query && query.practiceName && query.practiceAddress && query.odsCode) {
      const cookies = this.$store.$cookies;
      const { odsCode, practiceName, practiceAddress } = query;
      this.practiceName = practiceName;
      this.practiceAddress = practiceAddress;

      const betaCookie = Object.assign(
        {},
        cookies.get('BetaCookie'),
        {
          ODSCode: odsCode,
          PracticeName: practiceName,
          PracticeAddress: practiceAddress,
          PracticeParticipating: this.participating,
          Complete: true,
        },
      );

      cookies.set('BetaCookie', betaCookie, { path: '/', maxAge: moment.duration(1, 'y').asSeconds(), secure: this.$env.SECURE_COOKIES });

      if (process.client) {
        NativeCallbacks.storeBetaCookie();
      }
    } else {
      router.push({ path: GP_FINDER.path, query: { reset: true } });
    }
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/buttons';
@import "../../style/headerslim";
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfinderparticipation';

</style>
