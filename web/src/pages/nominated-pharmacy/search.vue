<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <div :class="$style.info">
      <p> {{ $t('searchNominatedPharmacy.line1') }} </p>
      <h3> {{ $t('searchNominatedPharmacy.subHeader') }} </h3>
      <p> {{ $t('searchNominatedPharmacy.line2') }} </p>
      <error-message v-if="showError"
                     :id="$style['error-label']"
                     role="alert">
        {{ $t('searchNominatedPharmacy.emptySearchError') }}
      </error-message>
      <form @submit.prevent="searchFormSubmitted">
        <generic-text-input id="searchTextInput"
                            v-model="searchQuery"
                            :class="$style.inputSpacing"
                            type="text"
                            name="searchQuery"
                            maxlength="10"/>
        <generic-button :button-classes="['green', 'button']">
          {{ $t('searchNominatedPharmacy.searchButton') }}
        </generic-button>
        <analytics-tracked-tag :text="$t('th03.errors.backButton')">
          <generic-button
            id="back-button"
            :button-classes="['grey', 'button']" :class="$style['back']"
            tabindex="0" @click.prevent="cancelButtonClicked">
            {{ $t('th03.errors.backButton') }}
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
import AbbreviationsArrowRightIcon from '@/components/icons/AbbreviationsArrowRightIcon';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_SEARCH_RESULTS } from '@/lib/routes';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    GenericTextInput,
    AbbreviationsArrowRightIcon,
    AnalyticsTrackedTag,
    ErrorMessage,
  },
  data() {
    return {
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
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    async searchClicked() {
      if (this.isButtonDisabled) return;
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
      this.goToUrl(NOMINATED_PHARMACY_SEARCH_RESULTS.path);

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
      return searchQuery && searchQuery.length >= 1 && searchQuery.length <= 10;
    },
    async searchForPharmacies(searchQuery) {
      const pharmacySearchResult = {
        noResultsFound: false,
        technicalError: false,
        pharmacies: [],
      };

      const pharmacySearchParams = {
        postcode: searchQuery,
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

.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}
.inputSpacing{
 margin-bottom: 1em;
}
</style>
