<template>
  <div :class="[getHeaderState(), 'pull-content', $store.state.device.isNativeApp && $style.web]">
    <div>
      <h2>{{ `${$t('th04.featuresHeader')} ${practiceName}` }}</h2>
      <analytics-tracked-tag :text="$t('th04.ctaNotMySurgery')"
                             :click-func="backButtonClicked"
                             :class="$style.linkBack"
                             tag="a">
        {{ $t('th04.ctaNotMySurgery') }}
      </analytics-tracked-tag>

      <hr>

      <div>
        <h2>{{ $t('th04.currentlyAvailableHeader') }}</h2>
        <ul id="availableFeatures" :class="$style.availableFeatures">
          <li v-for="(feature, index) in availableFeatures" :key="`feature-${index}`">
            {{ feature }}
          </li>
        </ul>
      </div>

      <div v-if="unavailableFeatures">
        <h2>{{ $t('th04.comingSoonHeader') }}</h2>
        <ul id="unavailableFeatures" :class="$style.unavailableFeatures">
          <li v-for="(feature, index) in unavailableFeatures" :key="`feature-${index}`">
            {{ feature }}
          </li>
        </ul>
      </div>

      <p v-if="practiceParticipating" id="createAccountMessage"
         :class="$style.createAccountMessage">
        {{ $t('th04.createAccountMessage') }}
      </p>

      <form v-if="practiceParticipating" :action="authoriseUrl" method="get">
        <input :value="scope" type="hidden" name="scope">
        <input :value="clientId" type="hidden" name="client_id">
        <input :value="codeChallenge" type="hidden" name="code_challenge">
        <input :value="codeChallengeMethod" type="hidden" name="code_challenge_method">
        <input :value="redirectUri" type="hidden" name="redirect_uri">
        <input :value="state" type="hidden" name="state">
        <input :value="responseType" type="hidden" name="response_type">
        <analytics-tracked-tag :text="this.$t('th04.ctaContinue')">
          <generic-button :class="$style.continue" :button-classes="[$store.state.device.isNativeApp
            ?'button':'button-desktop', 'green']">
            {{ this.$t('th04.ctaContinue') }}
          </generic-button>
        </analytics-tracked-tag>
      </form>

      <analytics-tracked-tag v-else :text="this.$t('th04.ctaContinue')">
        <generic-button :button-classes="[$store.state.device.isNativeApp
                          ?'button':'button-desktop', 'green',]" :class="$style.continue"
                        @click="notParticipatingCTAClicked">
          {{ this.$t('th04.ctaContinue') }}
        </generic-button>
      </analytics-tracked-tag>

      <p v-if="practiceParticipating" id="limitingFeaturesWarning">
        {{ $t('th04.limitingFeaturesWarning') }}
      </p>
    </div>
  </div>
</template>
<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import NativeCallbacks from '@/services/native-app';
import HeaderSlim from '@/components/HeaderSlim';
import BackIcon from '@/components/icons/BackIcon';
import GenericButton from '@/components/widgets/GenericButton';
import { GP_FINDER, BEGINLOGIN, GP_FINDER_SENDING_EMAIL } from '@/lib/routes';
import AuthorisationService from '@/services/authorisation-service';
import get from 'lodash/fp/get';

export default {
  components: {
    HeaderSlim,
    AnalyticsTrackedTag,
    BackIcon,
    GenericButton,
  },
  data() {
    return {
      defaultFeatures: this.$t('th04.defaultFeatures'),
      conditionalFeatures: this.$t('th04.conditionalFeatures'),
      organDonationNotParticipating: this.$t('th04.organDonationNotParticipating'),
      authoriseUrl: BEGINLOGIN.path,
      scope: '',
      codeChallenge: '',
      codeChallengeMethod: '',
      redirectUri: '',
      state: '',
      responseType: '',
      clientId: '',
    };
  },
  computed: {
    getHeaderText() {
      return this.$store.state.header.headerText;
    },
    availableFeatures() {
      return this.practiceParticipating ?
        [...this.defaultFeatures, ...this.conditionalFeatures] :
        [...this.defaultFeatures, ...this.organDonationNotParticipating];
    },
    unavailableFeatures() {
      return !this.practiceParticipating ? this.conditionalFeatures : undefined;
    },
    practiceParticipating() {
      return get('PracticeParticipating')(this.$store.state.throttling.selectedGpPractice);
    },
    practiceName() {
      return get('PracticeName')(this.$store.state.throttling.selectedGpPractice);
    },
  },
  asyncData({ store }) {
    if (get('PracticeParticipating')(store.state.throttling.selectedGpPractice)) {
      const authorisationService = new AuthorisationService(store.app.$env);
      return authorisationService.generateLoginUrl({
        source: store.state.device.source,
        cookies: store.$cookies,
      }).request;
    }
    return {};
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.showHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
    } else {
      window.scrollTo(0, 0);
    }
    if (!get('selectedGpPractice.ODSCode')(this.$store.state.throttling)) {
      this.goToUrl(GP_FINDER.path);
    }
  },
  methods: {
    getHeaderState() {
      return !this.$store.state.device.isNativeApp
        ? this.$style.webHeader : this.$style.nativeHeader;
    },
    notParticipatingCTAClicked() {
      this.goToUrl(GP_FINDER_SENDING_EMAIL.path);
    },
    backButtonClicked() {
      this.$store.dispatch('throttling/init');
      this.goToUrl(GP_FINDER.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfinderparticipation';
.webHeader {
  &.web {
    margin-top: -3.625em;
  }
}

.nativeHeader {
  padding: 0 0 3.125em 2.0px;
}
.throttlingContent {
  padding-top:0;
  padding-left:0;
}

</style>
