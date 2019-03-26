<template>
  <div :class="[$style.content, 'pull-content']">
    <div v-if="noResultsFound">
      <h2>
        {{ $t('nominatedPharmacySearchResults.errors.noResultsFound.header') }}
      </h2>
      <p> {{ foundNoResults }} </p>
    </div>
    <ul v-if="!noResultsFound" id="searchResults" :class="$style['list-menu']">
      <h2>{{ foundResults }}</h2>
      <p>{{ $t('nominatedPharmacySearchResults.resultSummary.showingAll') }}</p>
      <li v-for="pharmacy in pharmacies" :key="`pharmacy-${pharmacy.odsCode}`">
        <analytics-tracked-tag :id="`btnPharmacy-${pharmacy.odsCode}`"
                               :class="$style['no-decoration']"
                               text="Pharmacy"
                               tag="a"
                               href="#"
                               @click.native="pharmacyPracticeClicked(pharmacy)">
          <span :class="$style.fieldName">
            {{ pharmacy.pharmacyName }}
          </span>
          <p>
            {{ pharmacy.formattedAddress = formatAddress(pharmacy) }}
          </p>
          <p v-if="pharmacy.telephoneNumber">
            {{ pharmacy.telephoneNumber }}
          </p>
          <p v-if="pharmacy.distance !== null">
            {{ formatDistance(pharmacy.distance) }}
          </p>
        </analytics-tracked-tag>
      </li>
    </ul>
    <analytics-tracked-tag :text="$t('nominatedPharmacySearchResults.backButton')">
      <generic-button :button-classes="['grey', 'button']" :class="$style.back"
                      tabindex="0" @click="backButtonClicked">
        {{ $t('nominatedPharmacySearchResults.backButton') }}
      </generic-button>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import BackIcon from '@/components/icons/BackIcon';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_CONFIRM } from '@/lib/routes';

export default {
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
    const { searchResults, searchQuery } = this.$store.state.nominatedPharmacy;
    const { pharmacies, technicalError, noResultsFound } = searchResults;
    return {
      technicalError,
      noResultsFound,
      pharmacies,
      searchQuery,
    };
  },
  computed: {
    showPharmacies() {
      return !this.noResultsFound;
    },
    getHeaderText() {
      let header = this.$t('nominatedPharmacySearchResults.header');

      if (this.noResultsFound) {
        header = this.$t('nominatedPharmacySearchResults.errors.noResultsFound.header');
      }

      return header;
    },
    getTitle() {
      let title = this.$t('nominatedPharmacySearchResults.title');

      if (this.noResultsFound) {
        title = this.$t('nominatedPharmacySearchResults.errors.noResultsFound.title');
      }

      return title;
    },
    foundResults() {
      return this.$t('nominatedPharmacySearchResults.resultSummary.showingPharmaciesNear').replace('{searchQuery}', this.searchQuery);
    },
    foundNoResults() {
      return this.$t('nominatedPharmacySearchResults.errors.noResultsFound.foundNoResults').replace('{searchQuery}', this.searchQuery);
    },
  },
  mounted() {
    if (!this.searchQuery || (!this.pharmacies && !this.technicalError && !this.noResultsFound)) {
      this.goToUrl(NOMINATED_PHARMACY_SEARCH.path);
    }
  },
  methods: {
    formatAddress(pharmacy) {
      return [pharmacy.addressLine1, pharmacy.addressLine2, pharmacy.addressLine3, pharmacy.city,
        pharmacy.county, pharmacy.postcode].filter(Boolean).join(', ');
    },
    formatDistance(distance) {
      return this.$t('nominatedPharmacySearchResults.distanceAway').replace('{distance}', distance);
    },
    async pharmacyPracticeClicked(pharmacy) {
      this.$store.dispatch('nominatedPharmacy/select', pharmacy);
      this.goToUrl(NOMINATED_PHARMACY_CONFIRM.path);
    },
    backButtonClicked() {
      this.goToUrl(NOMINATED_PHARMACY_SEARCH.path);
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
