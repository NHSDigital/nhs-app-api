<template>
  <div>

    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ getHeaderText }} </h1>
      <analytics-tracked-tag text="back">
        <button @click="backButtonClicked">
          <back-icon/>
        </button>
      </analytics-tracked-tag>
    </header>

    <div v-if="showTemplate" :class="[$style.webHeader, $style.throttlingContent, 'pull-content']">

      <h2>{{ `${$t('th04.featuresHeader')} ${practiceName}` }}</h2>
      <analytics-tracked-tag :text="$t('th04.ctaNotMySurgery')"
                             :click-func="backButtonClicked"
                             :class="$style.linkBack"
                             tag="a">
        {{ $t('th04.ctaNotMySurgery') }}
      </analytics-tracked-tag>

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
          <generic-button :class="$style.continue" :button-classes="['green', 'button']">
            {{ this.$t('th04.ctaContinue') }}
          </generic-button>
        </analytics-tracked-tag>
      </form>

      <analytics-tracked-tag v-else :text="this.$t('th04.ctaContinue')">
        <generic-button :button-classes="['green', 'button']" :class="$style.continue"
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
import BackIcon from '@/components/icons/BackIcon';
import GenericButton from '@/components/widgets/GenericButton';
import { GP_FINDER, BEGINLOGIN, GP_FINDER_SENDING_EMAIL } from '@/lib/routes';
import get from 'lodash/fp/get';

export default {
  layout: 'throttling',
  components: {
    AnalyticsTrackedTag,
    BackIcon,
    GenericButton,
  },
  data() {
    return {
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
    };
  },
  computed: {
    getHeaderText() {
      return this.$store.state.header.headerText;
    },
    availableFeatures() {
      return this.practiceParticipating ?
        this.defaultFeatures.concat(this.conditionalFeatures) : this.defaultFeatures;
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
  mounted() {
    if (!this.$store.state.throttling.selectedGpPractice) {
      this.$store.dispatch('throttling/init');
      this.goToUrl(GP_FINDER.path);
    }
  },
  methods: {
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
@import "../../style/headerslim";
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfinderparticipation';
</style>
