<template>
  <div>

    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ getHeaderText }} </h1>
      <form :action="backLink" method="get">
        <input :value="true" type="hidden" name="reset">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <button type="submit">
          <back-icon/>
        </button>
      </form>
    </header>

    <div v-if="showTemplate" :class="[$style.webHeader, $style.throttlingContent, 'pull-content']">

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
          <p> {{ $t('th03.errors.serviceUnavailable.errorDialogText') }} </p>
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
        <hr>
      </div>

      <ul v-if="!technicalError && !noResultsFound" id="searchResults" :class="$style['list-menu']">
        <li v-for="gpPractice in gpPractices"
            :key="`gpPractice-${gpPractice.NACSCode}`">
          <analytics-tracked-tag :id="`btnGpPractice-${gpPractice.NACSCode}`"
                                 :class="$style['no-decoration']"
                                 :href="getHrefForGpPractice(gpPractice)"
                                 text=""
                                 tag="a">
            <span :class="$style.fieldName">
              {{ gpPractice.OrganisationName }}
            </span>
            <p> {{ formatAddress(gpPractice) }} </p>
          </analytics-tracked-tag>
        </li>
      </ul>

      <form v-if="tooManyResults || technicalError || noResultsFound"
            :action="backLink"
            method="get">
        <input :value="true" type="hidden" name="reset">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <generic-button :class="[$style.button, $style.grey, $style.back]"
                        tabindex="0"
                        type="submit">
          {{ $t('th03.errors.backButton') }}
        </generic-button>
      </form>

      <p v-if="technicalError">
        {{ $t('th03.errors.serviceUnavailable.mainContent') }}
      </p>

    </div>
  </div>
</template>

<script>
/* eslint-disable global-require */
/* eslint-disable dot-notation */
import BackIcon from '@/components/icons/BackIcon';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import NHSSearchService from '@/services/nhs-search-service';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import { GP_FINDER, GP_FINDER_PARTICIPATION } from '@/lib/routes';

export default {
  layout: 'throttling',
  components: {
    BackIcon,
    AnalyticsTrackedTag,
    GenericButton,
    MessageDialog,
  },
  head() {
    return {
      title: `${this.getTitle} - ${this.$t('appTitle')}`,
    };
  },
  data() {
    return {
      searchQuery: undefined,
      searchResults: [],
      tooManyResults: false,
      noResultsFound: false,
      technicalError: false,
      backLink: GP_FINDER.path,
    };
  },
  computed: {
    showGpPractices() {
      return this.searchResults && this.searchResults.length > 0;
    },
    gpPractices() {
      return this.searchResults ? this.searchResults : undefined;
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
  asyncData({ route, redirect }) {
    const { searchQuery } = route.query;
    if (!searchQuery || typeof searchQuery !== 'string' || !searchQuery.trim()) {
      return redirect(`${GP_FINDER.path}?error=true`);
    }

    const serviceResponse = NHSSearchService.searchGPPractices(searchQuery);
    if (serviceResponse.then) {
      return serviceResponse.then((response) => {
        if (response.postcodeSearchError) {
          return { technicalError: true };
        }
        if (response.noResultsFound) {
          return { noResultsFound: true, searchQuery };
        }
        return {
          tooManyResults: response.data['@odata.count'] > process.env['GP_LOOKUP_API_RESULTS_LIMIT'],
          noResultsFound: !response.data['@odata.count'],
          searchResults: response.data.value,
          searchQuery,
        };
      })
        .catch((error) => {
          if (process.server) {
            const consola = require('consola');
            consola.error(new Error(`Error searching for GP practice: response: ${error}`));
          }
          return { technicalError: true };
        });
    }
    if (serviceResponse.queryError) {
      return redirect(`${GP_FINDER.path}?error=true`);
    }
    return { technicalError: true };
  },
  methods: {
    formatAddress(gpPractice) {
      return [gpPractice.Address1, gpPractice.Address2, gpPractice.Address3, gpPractice.City,
        gpPractice.County, gpPractice.Postcode].filter(Boolean).join(', ');
    },
    getHrefForGpPractice(gpPractice) {
      return `${GP_FINDER_PARTICIPATION.path}?odsCode=${this.getPracticeCodeFromNACSCode(gpPractice.NACSCode)}` +
        `&practiceName=${encodeURIComponent(gpPractice.OrganisationName)}` +
        `&practiceAddress=${encodeURIComponent(this.formatAddress(gpPractice))}` +
        `&source=${this.$store.state.device.source}`;
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
