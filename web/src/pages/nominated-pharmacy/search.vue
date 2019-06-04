<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <div :class="$style.info">
      <message-dialog v-if="isInternetPharmacy"
                      id="warning-dialog"
                      message-type="warning"
                      icon-text="Important">
        <message-text id="warning-text"
                      :class="$style.warningText">
          {{ warningText }}
        </message-text>
      </message-dialog>
      <p> {{ $t('nominated_pharmacy.search.line1') }} </p>
      <h3> {{ $t('nominated_pharmacy.search.subHeader') }} </h3>
      <p id="pharmacy-search-label"
         :class="[$style['search-label-spacing'], $style['search-label']]">
        {{ $t('nominated_pharmacy.search.line2') }}
      </p>
      <error-message v-if="showError"
                     :id="$style['error-label']"
                     :class="$style['search-label-spacing']"
                     role="alert">
        {{ $t('nominated_pharmacy.search.emptySearchError') }}
      </error-message>
      <form @submit.prevent="searchFormSubmitted">
        <generic-text-input id="searchTextInput"
                            v-model="searchQuery"
                            :class="$style['input-spacing']"
                            type="text"
                            a-labelled-by="pharmacy-search-label"
                            name="searchQuery"
                            :maxlength="searchQueryMaxLengthAsString"/>
        <generic-button id="search-button" :button-classes="['green', 'button']" >
          {{ $t('nominated_pharmacy.search.searchButton') }}
        </generic-button>
        <analytics-tracked-tag :text="$t('generic.backButton.text')">
          <generic-button
            id="back-button"
            :button-classes="['grey', 'button']" :class="$style['back']"
            tabindex="0" @click.prevent="cancelButtonClicked">
            {{ $t('generic.backButton.text') }}
          </generic-button>
        </analytics-tracked-tag>
      </form>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { getDynamicStyle } from '@/lib/desktop-experience';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_SEARCH_RESULTS, PRESCRIPTIONS } from '@/lib/routes';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    GenericTextInput,
    AnalyticsTrackedTag,
    ErrorMessage,
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      searchQueryMaxLength: 150,
      isButtonDisabled: false,
      searchQuery: '',
      allPharmaciesURL: NOMINATED_PHARMACY_SEARCH.path,
      allDispensingContractorsURL: NOMINATED_PHARMACY_SEARCH.path,
      submissionError: false,
      backButtonPath: this.$store.getters['nominatedPharmacy/previousPage'],
    };
  },
  computed: {
    hasLoaded() {
      return this.$store.state.searchNominatedPharmacy.hasLoaded;
    },
    showError() {
      return this.submissionError;
    },
    searchQueryMaxLengthAsString() {
      return this.searchQueryMaxLength.toString();
    },
    isInternetPharmacy() {
      return (
        this.$store.state.nominatedPharmacy.pharmacy.pharmacySubType ===
          PharmacySubType.InternetPharmacy
      );
    },
    warningText() {
      return this.$t('nominated_pharmacy.warning.changeInternetPharmacy')
        .replace(/{pharmacyName}/g, this.$store.state.nominatedPharmacy.pharmacy.pharmacyName);
    },
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path, null);
    }
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    async searchClicked() {
      if (this.isButtonDisabled) {
        return;
      }
      this.isButtonDisabled = true;

      const processedQuery = this.processQuery(this.searchQuery);

      if (!this.validateSearchQuery(processedQuery)) {
        this.submissionError = true;
        this.isButtonDisabled = false;
        return;
      }
      const pharmacySearchResponse = await this.searchForPharmacies(processedQuery);

      if (pharmacySearchResponse.technicalError) {
        this.submissionError = true;
        this.isButtonDisabled = false;
        return;
      }

      this.$store.dispatch('nominatedPharmacy/setSearchQuery', processedQuery);
      this.$store.dispatch('nominatedPharmacy/setSearchResults', pharmacySearchResponse);
      redirectTo(this, NOMINATED_PHARMACY_SEARCH_RESULTS.path, null);

      this.isButtonDisabled = false;
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
    validateSearchQuery(searchQuery) {
      return searchQuery &&
             searchQuery.length >= 1 &&
             searchQuery.length <= this.searchQueryMaxLength;
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
      redirectTo(this, this.backButtonPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/spacings";
@import '../../style/colours';
@import '../../style/textstyles';

.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}

.input-spacing {
  margin-bottom: 1em;
}

.search-label-spacing {
  margin-bottom: 1em;
}

.search-label {
  @include small_text;
  color: $light_grey;
}
</style>
