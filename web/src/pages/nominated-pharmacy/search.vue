<template>
  <div v-if="showTemplate">
    <div v-if="foundNoResultsMessage" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>{{ foundNoResultsMessage }}</p>
        <h2>{{ $t('nominatedPharmacySearchResults.searchAgain') }}</h2>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="showErrors" message-type="error">
          <message-text data-purpose="error-heading">
            {{ $t('nominated_pharmacy.search.errorMessageHeader') }}
          </message-text>
          <message-list data-purpose="empty-search-error">
            <li id="empty-search-error">{{ $t('nominated_pharmacy.search.emptySearchError') }}</li>
          </message-list>
        </message-dialog>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <form @submit.prevent="searchFormSubmitted">
          <error-group :show-error="showErrors">
            <error-message v-if="showInvalidSearchError">
              {{ $t('nominated_pharmacy.search.emptySearchError') }}
            </error-message>
            <label id="pharmacy-search-label"
                   class="nhsuk-label"
                   for="searchTextInput">
              {{ $t('nominated_pharmacy.search.searchInputLabel') }}
            </label>
            <span id="pharmacy-search-hint"
                  class="nhsuk-hint">
              {{ $t('nominated_pharmacy.search.searchInputHint') }}
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
            {{ $t('nominated_pharmacy.search.searchButton') }}
          </generic-button>
          <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                                 :text="$t('generic.backButton.text')"
                                 :tabindex="-1">
            <desktopGenericBackLink id="back-link"
                                    :path="backButtonPath"
                                    :button-text="'generic.backButton.text'"
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
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_SEARCH_RESULTS, PRESCRIPTIONS } from '@/lib/routes';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';

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
      allPharmaciesURL: NOMINATED_PHARMACY_SEARCH.path,
      allDispensingContractorsURL: NOMINATED_PHARMACY_SEARCH.path,
      submissionError: false,
      showInvalidSearchError: false,
      backButtonPath: this.$store.getters['nominatedPharmacy/previousPage'],
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
      redirectTo(this, PRESCRIPTIONS.path);
    }
  },
  methods: {
    generateNoResultsMessage() {
      return this.$t('nominatedPharmacySearchResults.errors.noResultsFound.foundNoResults').replace('{searchQuery}', this.searchQuery);
    },
    async searchClicked() {
      this.showInvalidSearchError = false;

      const processedQuery = this.processQuery(this.searchQuery);

      if (!this.validateSearchQueryLength(processedQuery)
          || !this.validateSearchQueryPattern(processedQuery)) {
        this.showInvalidSearchError = true;
        window.scrollTo(0, 0);
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
        this.$store.dispatch('header/updateHeaderText', this.$t('nominatedPharmacySearchResults.errors.noResultsFound.header', { searchQuery: this.searchQuery }));
        this.$store.dispatch('pageTitle/updatePageTitle', this.$t('nominatedPharmacySearchResults.errors.noResultsFound.title', { searchQuery: this.searchQuery }));
        window.scrollTo(0, 0);
      } else {
        redirectTo(this, NOMINATED_PHARMACY_SEARCH_RESULTS.path);
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
          pharmacySearchResult.pharmacies = response;
          pharmacySearchResult.noResultsFound = pharmacySearchResult.pharmacies.length === 0;
        })
        .catch(() => {
          pharmacySearchResult.technicalError = true;
        });

      return pharmacySearchResult;
    },
    cancelButtonClicked() {
      // todo: needs to be new page
      redirectTo(this, this.backButtonPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
</style>
