<template>
  <div :class="[$style['pull-content'], $style.content,
                !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div v-if="noResultsFound" :class="$style.resultPanel">
      <p> {{ foundNoResults }} </p>
      <p>{{ $t('nominatedPharmacySearchResults.errors.noResultsFound.message') }}</p>
    </div>
    <div v-if="!noResultsFound" :class="$style.resultPanel">
      <div>{{ foundResults }}</div>
      <div>{{ $t('nominatedPharmacySearchResults.resultSummary.distanceInformation') }}</div>
      <ul id="searchResults"
          :class="[$style['list-menu-white'], $style.resultList]">
        <li v-for="pharmacy in pharmacies"
            :key="`pharmacy-${pharmacy.odsCode}`"
            :class="$style.link">
          <analytics-tracked-tag :id="`btnPharmacy-${pharmacy.odsCode}`"
                                 :class="!$store.state.device.isNativeApp ?
                                   [$style['no-decoration','pharmacy-link']] : ''"
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
    <analytics-tracked-tag :text="$t('nominatedPharmacySearchResults.backButton')"
                           :tabindex="-1">
      <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                              :path="searchNominatedPharmacyPath"
                              :button-text="'nominatedPharmacyNotFound.backButton'"
                              @clickAndPrevent="backButtonClicked"/>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_CONFIRM, PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    DesktopGenericBackLink,
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
    searchNominatedPharmacyPath() {
      return NOMINATED_PHARMACY_SEARCH.path;
    },
  },
  mounted() {
    if (!this.searchQuery || (!this.pharmacies && !this.technicalError && !this.noResultsFound)) {
      redirectTo(this, this.searchNominatedPharmacyPath);
    }
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path);
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
      redirectTo(this, NOMINATED_PHARMACY_CONFIRM.path);
    },
    backButtonClicked() {
      redirectTo(this, this.searchNominatedPharmacyPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import "../../style/colours";

div {
  &.desktopWeb {
  max-width: 540px;

  li {
    font-family: $default_web;
    font-weight: normal;
  }

  p {
    font-family: $default_web;
    font-weight: normal;
    }
  }
}
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

.pharmacy-link {
  // added to show the border on focus and hover
  position: relative;
  z-index: 1;
}
</style>
