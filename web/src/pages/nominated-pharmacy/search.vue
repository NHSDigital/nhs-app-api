<template>
  <div v-if="showTemplate">
    <div v-if="foundNoResultsMessage" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>{{ foundNoResultsMessage }}</p>
        <h2>{{ $t('nominatedPharmacy.searchResults.searchAgain') }}</h2>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div role="alert" aria-atomic="true">
          <message-dialog v-if="showErrors" message-type="error" :focusable="true">
            <message-text data-purpose="error-heading">
              {{ $t('nominatedPharmacy.search.errorMessageHeader') }}
            </message-text>
            <message-list data-purpose="empty-search-error">
              <li id="empty-search-error">{{ $t('nominatedPharmacy.search.emptySearchError') }}</li>
            </message-list>
          </message-dialog>
        </div>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <form @submit.prevent="searchFormSubmitted">
          <error-group :show-error="showErrors">
            <error-message v-if="showInvalidSearchError">
              {{ $t('nominatedPharmacy.search.emptySearchError') }}
            </error-message>
            <label id="pharmacy-search-label"
                   class="nhsuk-label"
                   for="searchTextInput">
              {{ $t('nominatedPharmacy.search.searchInputLabel') }}
            </label>
            <span id="pharmacy-search-hint"
                  class="nhsuk-hint">
              {{ $t('nominatedPharmacy.search.searchInputHint') }}
            </span>
            <generic-text-input id="searchTextInput"
                                v-model="searchQuery"
                                class="nhsuk-input--width-10 nhsuk-u-padding-bottom-3"
                                type="text"
                                a-labelled-by="pharmacy-search-label"
                                name="searchQuery"
                                :maxlength="searchQueryMaxLengthAsString"/>
          </error-group>
          <generic-button id="search-button"
                          :button-classes="['nhsuk-button']">
            {{ $t('nominatedPharmacy.search.searchButton') }}
          </generic-button>
          <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                                 :text="$t('generic.back')">
            <desktopGenericBackLink id="back-link"
                                    :path="backButtonPath"
                                    :button-text="'generic.back'"
                                    @clickAndPrevent="cancelButtonClicked"/>
          </analytics-tracked-tag>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import ErrorGroup from '@/components/ErrorGroup';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import {
  NOMINATED_PHARMACY_SEARCH_PATH,
  NOMINATED_PHARMACY_SEARCH_RESULTS_PATH,
  PRESCRIPTIONS_PATH,
  NOMINATED_PHARMACY_CHOOSE_TYPE_PATH,
} from '@/router/paths';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';
import { UPDATE_HEADER, UPDATE_TITLE, FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';

// If changing regex, ensure backend regex is updated.
const postcodeRegex = /^((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))( ?[0-9][A-Za-z]{2})?)$/;

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    GenericTextInput,
    AnalyticsTrackedTag,
    ErrorGroup,
    MessageDialog,
    MessageList,
    MessageText,
    ErrorMessage,
    DesktopGenericBackLink,
  },
  data() {
    return {
      searchQueryMaxLength: 10,
      searchQuery: '',
      allPharmaciesURL: NOMINATED_PHARMACY_SEARCH_PATH,
      allDispensingContractorsURL: NOMINATED_PHARMACY_SEARCH_PATH,
      submissionError: false,
      showInvalidSearchError: false,
      backButtonPath: NOMINATED_PHARMACY_CHOOSE_TYPE_PATH,
      foundNoResultsMessage: '',
    };
  },
  computed: {
    hasLoaded() {
      return this.$store.state.searchNominatedPharmacy.hasLoaded;
    },
    searchQueryMaxLengthAsString() {
      return this.searchQueryMaxLength.toString();
    },
    showErrors() {
      return this.showInvalidSearchError;
    },
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS_PATH);
    }
  },
  methods: {
    generateNoResultsMessage() {
      return this.$t('nominatedPharmacy.searchResults.errors.noResultsFound.foundNoResults').replace('{searchQuery}', this.searchQuery);
    },
    async searchClicked() {
      this.showInvalidSearchError = false;

      const processedQuery = this.processQuery(this.searchQuery);

      if (!this.validateSearchQueryLength(processedQuery)
          || !this.validateSearchQueryPattern(processedQuery)) {
        this.$nextTick(() => {
          this.showInvalidSearchError = true;
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
        });
        return;
      }

      const pharmacySearchResponse = await this.searchForPharmacies(processedQuery);

      if (pharmacySearchResponse.technicalError) {
        this.submissionError = true; // there doesn't seem to be any UI element for this
        return;
      }

      this.$store.dispatch('nominatedPharmacy/setSearchQuery', processedQuery);
      this.$store.dispatch('nominatedPharmacy/setSearchResults', pharmacySearchResponse);

      if (pharmacySearchResponse.noResultsFound) {
        this.foundNoResultsMessage = this.generateNoResultsMessage();
        const formatArgs = { searchQuery: this.searchQuery };
        EventBus.$emit(UPDATE_HEADER, this.$t('nominatedPharmacy.searchResults.errors.noResultsFound.header', formatArgs), true);
        EventBus.$emit(UPDATE_TITLE, this.$t('nominatedPharmacy.searchResults.errors.noResultsFound.title', formatArgs), true);
        window.scrollTo(0, 0);
      } else {
        redirectTo(this, NOMINATED_PHARMACY_SEARCH_RESULTS_PATH);
      }
    },
    searchFormSubmitted() {
      this.searchClicked();
    },
    processQuery(searchQuery) {
      let processedQuery;

      if (searchQuery) {
        processedQuery = searchQuery.trim();
      }

      return processedQuery;
    },
    validateSearchQueryLength(searchQuery) {
      return searchQuery &&
          searchQuery.length >= 1 &&
          searchQuery.length <= this.searchQueryMaxLength;
    },
    validateSearchQueryPattern(searchQuery) {
      return postcodeRegex.test(searchQuery);
    },
    async searchForPharmacies(searchQuery) {
      const pharmacySearchResult = {
        noResultsFound: false,
        technicalError: false,
        pharmacies: [],
      };

      const pharmacySearchParams = {
        searchTerm: searchQuery,
      };

      await this.$store.app.$http.getV1PatientPharmacies(pharmacySearchParams)
        .then((response) => {
          pharmacySearchResult.pharmacies = response.pharmacies;
          pharmacySearchResult.noResultsFound =
            (pharmacySearchResult.pharmacies === undefined ||
            pharmacySearchResult.pharmacies.length === 0);
        })
        .catch(() => {
          pharmacySearchResult.technicalError = true;
        });

      return pharmacySearchResult;
    },
    cancelButtonClicked() {
      redirectTo(this, this.backButtonPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
</style>
