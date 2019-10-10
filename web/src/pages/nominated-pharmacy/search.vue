<template>
  <div v-if="showTemplate" :class="[$style['pull-content'], $style.content,
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div :class="$style.info">
      <message-dialog id="warning-dialog-nominated-pharmacy"
                      message-type="warning"
                      :icon-text="$t('messageIconText.important')">
        <div v-if="noPharmacyNominated">
          <message-text>
            {{ $t('nominated_pharmacy.search.line1') }}
          </message-text>
        </div>
        <div v-else-if="isInternetPharmacy">
          <message-text>
            {{ warningText }}
          </message-text>
        </div>
        <div v-else>
          <message-text>
            {{ $t('nominated_pharmacy.search.warning.paragraph1') }}
          </message-text>
          <message-text>
            {{ $t('nominated_pharmacy.search.warning.paragraph2') }}
          </message-text>
        </div>
      </message-dialog>
      <h3> {{ $t('nominated_pharmacy.search.subHeader') }} </h3>
      <p id="pharmacy-search-label"
         :class="[$style['search-label-spacing'], $style['search-label']]">
        {{ $t('nominated_pharmacy.search.line2') }}
      </p>
      <error-message v-if="showEmptySearchError"
                     :id="$style['error-label']"
                     :class="$style['search-label-spacing']"
                     role="alert">
        {{ $t('nominated_pharmacy.search.emptySearchError') }}
      </error-message>
      <form @submit.prevent="searchFormSubmitted">
        <generic-text-input id="searchTextInput"
                            v-model="searchQuery"
                            :class="[$store.state.device.isNativeApp
                              ? $style['input-spacing']: $style['input-spacing-desktop']]"
                            type="text"
                            a-labelled-by="pharmacy-search-label"
                            name="searchQuery"
                            :maxlength="searchQueryMaxLengthAsString"/>
        <generic-button id="search-button"
                        :button-classes="['nhsuk-button']">
          {{ $t('nominated_pharmacy.search.searchButton') }}
        </generic-button>
        <analytics-tracked-tag :text="$t('generic.backButton.text')" :tabindex="-1">
          <generic-button v-if="$store.state.device.isNativeApp"
                          id="back-button"
                          :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                          tabindex="0" @click.prevent="cancelButtonClicked">
            {{ $t('generic.backButton.text') }}
          </generic-button>
          <desktopGenericBackLink v-else
                                  id="back-link"
                                  :path="backButtonPath"
                                  :button-text="'generic.backButton.text'"
                                  @clickAndPrevent="cancelButtonClicked"/>
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
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    GenericTextInput,
    AnalyticsTrackedTag,
    ErrorMessage,
    MessageDialog,
    MessageText,
    DesktopGenericBackLink,
  },
  data() {
    return {
      searchQueryMaxLength: 10,
      searchQuery: '',
      allPharmaciesURL: NOMINATED_PHARMACY_SEARCH.path,
      allDispensingContractorsURL: NOMINATED_PHARMACY_SEARCH.path,
      submissionError: false,
      showEmptySearchError: false,
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
    noPharmacyNominated() {
      return (
        this.$store.state.nominatedPharmacy.pharmacy.pharmacyName ===
          undefined
      );
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
      this.showEmptySearchError = false;

      const processedQuery = this.processQuery(this.searchQuery);

      if (!this.validateSearchQueryLength(processedQuery)) {
        this.showEmptySearchError = true;
        return;
      }
      const pharmacySearchResponse = await this.searchForPharmacies(processedQuery);

      if (pharmacySearchResponse.technicalError) {
        this.submissionError = true; // there doesn't seem to be any UI element for this
        return;
      }

      this.$store.dispatch('nominatedPharmacy/setSearchQuery', processedQuery);
      this.$store.dispatch('nominatedPharmacy/setSearchResults', pharmacySearchResponse);
      redirectTo(this, NOMINATED_PHARMACY_SEARCH_RESULTS.path, null);
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
  margin-top: 1em;
  margin-bottom: 1em;
}

.input-spacing-desktop {
  margin-top: 1em;
  margin-bottom: 1em;
  max-width: 350px;
}

.search-label-spacing {
  margin-bottom: 1em;
}

.search-label {
  @include small_text;
  color: $light_grey;
}
</style>
