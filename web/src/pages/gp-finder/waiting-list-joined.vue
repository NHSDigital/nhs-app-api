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
      <h2>{{ this.$t('th06.whatHappensNextHeading') }}</h2>
      <p v-if="joined">{{ this.$t('th06.whatHappensNextJoinedParagraph1') }}</p>
      <p v-if="joined">{{ this.$t('th06.whatHappensNextJoinedParagraph2') }}</p>
      <p v-else>{{ this.$t('th06.whatHappensNextNotJoinedParagraph') }}</p>
      <h2>{{ this.$t('th06.untilThenHeading') }}</h2>
      <p>{{ this.$t('th06.untilThenParagraph') }}</p>
      <analytics-tracked-tag :text="this.$t('th06.homeButton')">
        <generic-button :class="$style.goToHomeScreenButton"
                        :button-classes="['grey', 'button']"
                        @click="onReturnHomeClicked">
          {{ this.$t('th06.homeButton') }}
        </generic-button>
      </analytics-tracked-tag>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import BackIcon from '@/components/icons/BackIcon';
import HeaderSlim from '@/components/HeaderSlim';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import { LOGIN, GP_FINDER } from '@/lib/routes';
import { setCookie } from '@/lib/cookie-manager';
import NativeCallbacks from '@/services/native-app';
import get from 'lodash/fp/get';
import moment from 'moment';

export default {
  layout: 'throttling',
  components: {
    AnalyticsTrackedTag,
    HeaderSlim,
    BackIcon,
    GenericTextInput,
    GenericButton,
  },
  data() {
    return {
      joined: get('throttling.waitingListChoice')(this.$store.state) !== 'no',
    };
  },
  computed: {
    getHeaderText() {
      return this.$store.state.header.headerText;
    },
  },
  methods: {
    onReturnHomeClicked() {
      const betaCookie = this.$store.app.$cookies.get('BetaCookie');

      betaCookie.Complete = true;

      setCookie({
        cookies: this.$store.app.$cookies,
        key: 'BetaCookie',
        value: betaCookie,
        options: {
          maxAge: moment.duration(1, 'y').asSeconds(),
          secure: this.$store.app.$env.SECURE_COOKIES,
        },
      });

      if (process.client) {
        NativeCallbacks.storeBetaCookie();
      }

      this.goToUrl(LOGIN.path);
    },
    backButtonClicked() {
      this.goToUrl(GP_FINDER.path);
    },
  },
  beforeDestroy() {
    this.$store.dispatch('throttling/init');
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/elements';
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfindersendemail';
@import '../../style/headerslim';
</style>
