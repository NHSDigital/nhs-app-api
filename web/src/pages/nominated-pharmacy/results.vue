<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div>
        <p v-if="isHighStreetSearch" id="distance-information">
          {{ $t('nominatedPharmacySearchResults.resultSummary.distanceInformation') }}
        </p>
        <p v-else-if="!isOnlineWithSearch" id="random-results-information">
          {{ $t('nominatedPharmacySearchResults.online.random.information') }}
        </p>
        <menu-item-list id="searchResults">
          <menu-item v-for="(pharmacy, index) in pharmacies"
                     :id="'pharmacy-menu-item-' + index"
                     :key="`pharmacy-${pharmacy.odsCode}`"
                     header-tag="h2"
                     :target="'_blank'"
                     :text="pharmacy.pharmacyName"
                     :click-func="pharmacyPracticeClicked"
                     :click-param="pharmacy"
                     :aria-label="ariaLabelCaption(
                       `${pharmacy.pharmacyName}`,
                       `${formatAddress(pharmacy)}`,
                       `${formatTelephone(pharmacy.telephoneNumber)}`,
                       `${formatDistance(pharmacy.distance)}`)">
            <slot>
              <div class="nhsuk-u-padding-left-2" :class="$style['results-styling']">
                <pharmacy-address-component id="resultAddressComponent" :pharmacy="pharmacy"/>
              </div>
            </slot>
          </menu-item>
        </menu-item-list>
      </div>
      <analytics-tracked-tag :text="$t('nominatedPharmacySearchResults.backButton')"
                             :tabindex="-1">
        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="previousPagePath"
                                :button-text="'nominatedPharmacySearchResults.backButton'"
                                @clickAndPrevent="backButtonClicked"/>
      </analytics-tracked-tag>
    </div>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import {
  NOMINATED_PHARMACY_SEARCH,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH,
  NOMINATED_PHARMACY_CONFIRM,
  PRESCRIPTIONS,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES,
} from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import PharmacyAddressComponent from '@/components/nominatedPharmacy/PharmacyAddressComponent';

export default {
  layout: 'nhsuk-layout',
  components: {
    PharmacyAddressComponent,
    MenuItem,
    MenuItemList,
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
      isHighStreetSearch:
        this.$store.state.nominatedPharmacy.chosenType === PharmacyTypeChoice.HIGH_STREET_PHARMACY,
      isOnline:
        this.$store.state.nominatedPharmacy.chosenType === PharmacyTypeChoice.ONLINE_PHARMACY,
      isOnlineWithSearch:
        this.$store.state.nominatedPharmacy.onlineOnlyKnownOption,
    };
  },
  computed: {
    previousPagePath() {
      if (this.isHighStreetSearch) {
        return NOMINATED_PHARMACY_SEARCH.path;
      }
      if (this.isOnlineWithSearch) {
        return NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH.path;
      }
      return NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path;
    },
    searchNominatedPharmacyPath() {
      return NOMINATED_PHARMACY_SEARCH.path;
    },
  },
  mounted() {
    if (this.isHighStreetSearch && !this.searchQuery) {
      redirectTo(this, NOMINATED_PHARMACY_SEARCH.path);
    }
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']
        || !this.$store.state.nominatedPharmacy.chosenType) {
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
    formatTelephone(number) {
      return this.$t('nominatedPharmacySearchResults.telephoneLabel') + number;
    },
    async pharmacyPracticeClicked(pharmacy) {
      this.$store.dispatch('nominatedPharmacy/select', pharmacy);
      redirectTo(this, NOMINATED_PHARMACY_CONFIRM.path);
    },
    backButtonClicked() {
      redirectTo(this, this.previousPagePath);
    },
    ariaLabelCaption(header, address, telephone, distance) {
      return `${this.$t(header)}. ${this.$t(address)}. ${this.$t(telephone)}. ${this.$t(distance)}.`;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/listmenu';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  .results-styling {
    color: $color_nhsuk-black;
  }
</style>
