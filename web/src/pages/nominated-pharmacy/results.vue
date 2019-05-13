<template>
  <div :class="[$style.content, 'pull-content']">
    <div v-if="noResultsFound" :class="$style.resultPanel">
      <h2>
        {{ $t('nominatedPharmacySearchResults.errors.noResultsFound.header') }}
      </h2>
      <p> {{ foundNoResults }} </p>
      <h2>{{ $t('nominatedPharmacySearchResults.errors.noResultsFound.subHeader') }}</h2>
      <ul :class="$style.bullet">
        <li>{{ $t('nominatedPharmacySearchResults.errors.noResultsFound.message1') }}</li>
        <li>{{ $t('nominatedPharmacySearchResults.errors.noResultsFound.message2') }}</li>
      </ul>
    </div>
    <div v-if="!noResultsFound" :class="$style.resultPanel">
      <p>{{ foundResults }}</p>
      <ul id="searchResults"
          :class="[$style['list-menu-white'], $style.resultList]">
        <li v-for="pharmacy in pharmacies"
            :key="`pharmacy-${pharmacy.odsCode}`"
            :class="$style.link">
          <analytics-tracked-tag :id="`btnPharmacy-${pharmacy.odsCode}`"
                                 text="Pharmacy"
                                 tag="a"
                                 href="#"
                                 @click.native="pharmacyPracticeClicked(pharmacy)">
            <div>
              <p :class="$style.fieldName">
                {{ pharmacy.pharmacyName }}
              </p>
              <p>
                {{ pharmacy.formattedAddress = formatAddress(pharmacy) }}
              </p>
              <p v-if="pharmacy.telephoneNumber">
                {{ pharmacy.telephoneNumber }}
              </p>
              <p v-if="pharmacy.distance !== null">
                <b>{{ formatDistance(pharmacy.distance) }}</b>
              </p>
            </div>
          </analytics-tracked-tag>
        </li>
      </ul>
    </div>
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
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_CONFIRM, PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    BackIcon,
    GenericButton,
    MessageDialog,
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
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path, null);
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
@import "../../style/colours";

.resultPanel {
  margin-top: 1em;
  margin-bottom: 1em;
}
.resultList {
  margin-top: 1em;
}

.fieldName {
   padding-bottom: 1rem;
   color: $dark_grey;
   font-weight: 700;
}

.bullet {
  list-style-type: disc;
  padding-left: 1rem;
}
</style>
