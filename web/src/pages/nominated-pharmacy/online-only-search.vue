<template>
  <div v-if="showTemplate">
    <div v-if="foundNoResultsMessage" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p id="noResultsFoundText" class="nhsuk-u-padding-bottom-1">
          {{ foundNoResultsMessage }}
        </p>
        <h2 id="noResultsFoundHeader">
          {{ $t('nominatedPharmacyOnlineOnlySearch.searchAgainMessage') }}
        </h2>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="showErrors" id="error-message" message-type="error">
          <message-text data-purpose="error-heading">
            {{ $t('nominatedPharmacyOnlineOnlySearch.errorMessageHeader') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('nominatedPharmacyOnlineOnlySearch.errorMessageText') }}</li>
          </message-list>
        </message-dialog>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <form @submit.prevent="searchButtonClicked">
          <error-group :show-error="showErrors">
            <error-message v-if="showErrorMessage" id="invalid-search-term-error">
              {{ $t('nominatedPharmacyOnlineOnlySearch.errorMessageText') }}
            </error-message>
            <generic-text-input id="searchTextInput"
                                v-model="searchQuery"
                                class="nhsuk-input--width-20 nhsuk-u-padding-bottom-3"
                                type="text"
                                name="searchQuery"/>
          </error-group>
          <generic-button id="search-button"
                          :button-classes="['nhsuk-button']">
            {{ $t('nominatedPharmacyOnlineOnlySearch.searchButton') }}
          </generic-button>
        </form>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink id="back-link"
                                  :path="nominatedPharmacyChooseType"
                                  :button-text="'generic.backButton.text'"
                                  @clickAndPrevent="backButtonClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>

  </div>
</template>

<script>
import { NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES, NOMINATED_PHARMACY_SEARCH_RESULTS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import ErrorGroup from '@/components/ErrorGroup';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  name: 'OnlineOnlySearch',
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
      nominatedPharmacyChooseType: NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path,
      showErrorMessage: false,
      searchQuery: '',
      foundNoResultsMessage: '',
    };
  },
  computed: {
    showErrors() {
      return this.showErrorMessage;
    },
  },
  methods: {
    generateNoResultsMessage() {
      return this.$t('nominatedPharmacyOnlineOnlySearch.noResultsHelpText')
        .replace('{searchQuery}', this.processQuery(this.searchQuery));
    },
    backButtonClicked() {
      redirectTo(this, this.nominatedPharmacyChooseType);
    },
    async searchButtonClicked() {
      this.showErrorMessage = false;

      const processedQuery = this.processQuery(this.searchQuery);

      // Ensuring one character has been entered
      if (processedQuery.length === 0) {
        this.showErrorMessage = true;
      } else {
        const pharmacySearchResult = {
          noResultsFound: false,
          technicalError: false,
          pharmacies: [],
        };

        const pharmacySearchParams = {
          searchTerm: processedQuery,
        };

        try {
          const response =
            await this.$store.app.$http.getV1PatientOnlinePharmacies(pharmacySearchParams);

          pharmacySearchResult.pharmacies = response;
          pharmacySearchResult.noResultsFound = pharmacySearchResult.pharmacies.length === 0;
        } catch {
          pharmacySearchResult.technicalError = true;
        }

        if (pharmacySearchResult.technicalError) {
          this.submissionError = true;
          return;
        }

        this.$store.dispatch('nominatedPharmacy/setSearchQuery', processedQuery);
        this.$store.dispatch('nominatedPharmacy/setSearchResults', pharmacySearchResult);

        if (pharmacySearchResult.noResultsFound) {
          this.foundNoResultsMessage = this.generateNoResultsMessage();
          this.$store.dispatch('header/updateHeaderText', this.$t('nominatedPharmacySearchResults.errors.noResultsFound.header', { searchQuery: processedQuery }));
          this.$store.dispatch('pageTitle/updatePageTitle', this.$t('nominatedPharmacySearchResults.errors.noResultsFound.title', { searchQuery: processedQuery }));
          window.scrollTo(0, 0);
        } else {
          redirectTo(this, NOMINATED_PHARMACY_SEARCH_RESULTS.path);
        }
      }
    },
    processQuery(searchQuery) {
      let processedQuery = '';

      if (searchQuery) {
        processedQuery = searchQuery.trim();
      }

      return processedQuery;
    },
  },
};
</script>

<style scoped>

</style>
