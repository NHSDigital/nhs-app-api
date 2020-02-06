<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p id="confirm-help-text">{{ $t('nominated_pharmacy.confirm.confirmHelpText') }}</p>
        <div v-if="isOnlineOnlySelected">
          <online-only-pharmacy-detail :pharmacy="nominatedPharmacy"/>
        </div>
        <div v-else-if="isHighStreetSelected">
          <pharmacy-summary id="pharmacy-summary"
                            :pharmacy="nominatedPharmacy"/>
          <pharmacy-opening-times id="pharmacy-opening-times"
                                  :pharmacy-opening-time="nominatedPharmacy.openingTimesFormatted"/>
        </div>
        <generic-button id="confirm-button"
                        :button-classes="['nhsuk-button']"
                        @click.stop.prevent="submitNominatedPharmacy">
          {{ $t('nominated_pharmacy.confirm.confirmButton') }}
        </generic-button>
        <analytics-tracked-tag :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                  id="back-link"
                                  :path="nominatedPharmacySearchResultsPath"
                                  :button-text="'generic.backButton.text'"
                                  @clickAndPrevent="cancelButtonClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import OnlineOnlyPharmacyDetail from '@/components/nominatedPharmacy/OnlineOnlyPharmacyDetail';
import PharmacySummary from '../../components/nominatedPharmacy/PharmacySummary';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_SEARCH_RESULTS, PRESCRIPTIONS, NOMINATED_PHARMACY_CHANGE_SUCCESS } from '@/lib/routes';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    AnalyticsTrackedTag,
    OnlineOnlyPharmacyDetail,
    DesktopGenericBackLink,
    PharmacySummary,
    PharmacyOpeningTimes,
  },
  data() {
    return {
      openingTimes: this.$store.state.nominatedPharmacy
        .selectedNominatedPharmacy.openingTimesFormatted,
      nominatedPharmacy: this.$store.state.nominatedPharmacy.selectedNominatedPharmacy,
      nominatedPharmacySearchResultsPath: NOMINATED_PHARMACY_SEARCH_RESULTS.path,
      isHighStreetSelected:
        this.$store.state.nominatedPharmacy.chosenType === PharmacyTypeChoice.HIGH_STREET_PHARMACY,
      isOnlineOnlySelected:
        this.$store.state.nominatedPharmacy.chosenType === PharmacyTypeChoice.ONLINE_PHARMACY,
    };
  },
  created() {
    if (this.nominatedPharmacy === null) {
      redirectTo(this, PRESCRIPTIONS.path);
    }
  },
  methods: {
    async submitNominatedPharmacy() {
      try {
        await this.$store.dispatch('nominatedPharmacy/update', this.nominatedPharmacy.odsCode);
        redirectTo(this, NOMINATED_PHARMACY_CHANGE_SUCCESS.path);
      } catch (error) {
        /*
          empty catch block as the
          ApiError.vue (component) handles and
          surfaces appropriate error content based on the http status code returned from the API
          */
      }
    },
    cancelButtonClicked() {
      redirectTo(this, this.nominatedPharmacySearchResultsPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";

  div {
    &.desktopWeb {
      max-width: 540px;

      .warningText {
        font-family: $default_web;
        font-weight: normal;
      }

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
</style>
