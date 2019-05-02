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

    <div :class="[$style.webHeader, $style.throttlingContent, 'pull-content']">

      <div v-if="noResultsFound">
        <h2 :key="'noResultsFoundHeader'">
          {{ $t('th03.errors.noResultsFound.noResultsFoundHeader') }}
        </h2>
        <p :id="$style.errorContent" :key="'noResultsErrorContent'"> {{ foundNoResults }} </p>
        <h3 :key="'noResultsSuggestionHeader'">
          {{ $t('th03.errors.noResultsFound.suggestionHeader') }}
        </h3>
        <ul :class="$style.suggestions">
          <li v-for="(suggestion, index) in $t('th03.errors.noResultsFound.suggestions')"
              :key="`nrs-${index}`">
            {{ suggestion }}
          </li>
        </ul>
      </div>

      <div v-else-if="technicalError" :class="$style.technicalError">
        <message-dialog>
          <h3> {{ $t('th03.errors.serviceUnavailable.errorDialogHeader') }} </h3>
          <p> {{ $t('th03.errors.serviceUnavailable.errorDialogText1') }} </p>
          <p> {{ $t('th03.errors.serviceUnavailable.errorDialogText2') }} </p>
        </message-dialog>
      </div>

      <div v-else-if="tooManyResults" :class="$style.tooManyResultsError">
        <h2 :key="'tooManyResultsHeader'">
          {{ $t('th03.errors.tooManyResults.tooManyResultsHeader') }}
        </h2>
        <p :id="$style.errorContent" :key="'tooManyResultsErrorContent'">
          {{ $t('th03.errors.tooManyResults.foundTooManyResults') }}
        </p>
        <h3 :key="'tooManyResultsSuggestionHeader'">
          {{ $t('th03.errors.tooManyResults.suggestionHeader') }}
        </h3>
        <ul :class="$style.suggestions">
          <li v-for="(suggestion, index) in $t('th03.errors.tooManyResults.suggestions')"
              :key="`tmrs-${index}`">
            {{ suggestion }}
          </li>
        </ul>
        <hr aria-hidden="true">
      </div>

      <ul v-if="!technicalError && !noResultsFound" id="searchResults" :class="$style['list-menu']">
        <li v-for="gpPractice in gpPractices"
            :key="`gpPractice-${gpPractice.nacsCode}`">
          <analytics-tracked-tag :id="`btnGpPractice-${gpPractice.nacsCode}`"
                                 :class="$style['no-decoration']"
                                 text="GP practice"
                                 tag="a"
                                 href="#"
                                 @click.native="gpPracticeClicked(gpPractice)">
            <span :class="$style.fieldName">
              {{ gpPractice.organisationName }}
            </span>
            <p> {{ gpPractice.formattedAddress = formatAddress(gpPractice) }} </p>
          </analytics-tracked-tag>
        </li>
      </ul>

      <analytics-tracked-tag :text="$t('th03.errors.backButton')">
        <generic-button v-if="tooManyResults || technicalError || noResultsFound"
                        :button-classes="['grey', 'button']" :class="$style.back"
                        tabindex="0" @click="backButtonClicked">
          {{ $t('th03.errors.backButton') }}
        </generic-button>
      </analytics-tracked-tag>

    </div>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import BackIcon from '@/components/icons/BackIcon';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import { setCookie } from '@/lib/cookie-manager';
import { GP_FINDER, GP_FINDER_PARTICIPATION } from '@/lib/routes';
import NativeCallbacks from '@/services/native-app';
import moment from 'moment';

export default {
  layout: 'throttling',
  components: {
    AnalyticsTrackedTag,
    BackIcon,
    GenericButton,
    MessageDialog,
  },
  head() {
    return {
      title: `${this.getTitle} - ${this.$t('appTitle')}`,
    };
  },
  data() {
    const { searchResults, searchQuery } = this.$store.state.throttling;
    const { organisations, technicalError, noResultsFound, tooManyResults } = searchResults;
    return {
      technicalError,
      noResultsFound,
      tooManyResults,
      organisations,
      searchQuery,
      practiceClicked: false,
    };
  },
  computed: {
    showGpPractices() {
      return !this.noResultsFound;
    },
    gpPractices() {
      return this.organisations;
    },
    getHeaderText() {
      let header = this.$t('th03.header');

      if (this.noResultsFound) {
        header = this.$t('th03.errors.noResultsFound.header');
      } else if (this.technicalError) {
        header = this.$t('th03.errors.serviceUnavailable.header');
      }

      return header;
    },
    getTitle() {
      let title = this.$t('th03.title');

      if (this.noResultsFound) {
        title = this.$t('th03.errors.noResultsFound.title');
      } else if (this.technicalError) {
        title = this.$t('th03.errors.serviceUnavailable.title');
      }

      return title;
    },
    foundNoResults() {
      return this.$t('th03.errors.noResultsFound.foundNoResults').replace('{searchQuery}', this.searchQuery);
    },
  },
  mounted() {
    if (!this.searchQuery ||
      (!this.organisations && !this.technicalError &&
       !this.noResultsFound && !this.tooManyResults)) {
      this.goToUrl(GP_FINDER.path);
    }
  },
  methods: {
    formatAddress(gpPractice) {
      return [gpPractice.address1, gpPractice.address2, gpPractice.address3, gpPractice.city,
        gpPractice.county, gpPractice.postcode].filter(Boolean).join(', ');
    },
    async gpPracticeClicked(gpPractice) {
      if (this.practiceClicked) return;
      this.practiceClicked = true;

      const formattedGpPractice = {
        PracticeAddress: gpPractice.formattedAddress,
        PracticeName: gpPractice.organisationName,
        ODSCode: this.getPracticeCodeFromNACSCode(gpPractice.nacsCode),
      };

      const self = this;
      await this.$store.app.$http.getV1Odscodelookup({ odsCode: formattedGpPractice.ODSCode })
        .then((response) => {
          formattedGpPractice.PracticeParticipating = response && response.isGpSystemSupported;
        })
        .catch(() => {
          self.technicalError = true;
        });

      if (formattedGpPractice.PracticeParticipating !== undefined) {
        this.updateBetaCookie(formattedGpPractice);
        this.$store.dispatch('throttling/setSelectedGpPractice', formattedGpPractice);
        this.goToUrl(GP_FINDER_PARTICIPATION.path);
      }

      this.practiceClicked = false;
    },
    updateBetaCookie(gpPractice) {
      let betaCookie = this.$store.app.$cookies.get('BetaCookie');

      betaCookie = {
        ...betaCookie,
        ...gpPractice,
        Complete: true,
      };

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
    },
    backButtonClicked() {
      this.$store.dispatch('throttling/init');
      this.goToUrl(GP_FINDER.path);
    },
    getPracticeCodeFromNACSCode(nacsCode) {
      return nacsCode && nacsCode.length > 6 ?
        nacsCode.substring(0, 6) :
        nacsCode;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import '../../style/headerslim';
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfinderresults';
</style>
