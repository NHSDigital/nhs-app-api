<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <pharmacy-detail v-if="isHighStreetSelected" id="pharmacy-detail"
                         :pharmacy="nominatedPharmacy"
                         :is-my-nominated-pharmacy="false" />
        <online-only-pharmacy-detail v-else-if="isOnlineOnlySelected" id="online-pharmacy-detail"
                                     :pharmacy="nominatedPharmacy"/>
        <generic-button id="confirm-button"
                        :button-classes="['nhsuk-button']"
                        @click.stop.prevent="submitNominatedPharmacy">
          {{ $t('nominated_pharmacy.confirm.confirmButton') }}
        </generic-button>
        <analytics-tracked-tag :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <generic-button v-if="$store.state.device.isNativeApp" id="back-button"
                          :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                          tabindex="0" @click.prevent="cancelButtonClicked">
            {{ $t('generic.backButton.text') }}
          </generic-button>
          <desktopGenericBackLink v-else
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
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import OnlineOnlyPharmacyDetail from '@/components/nominatedPharmacy/OnlineOnlyPharmacyDetail';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_SEARCH_RESULTS, PRESCRIPTIONS, NOMINATED_PHARMACY_CHANGE_SUCCESS } from '@/lib/routes';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    AnalyticsTrackedTag,
    PharmacyDetail,
    OnlineOnlyPharmacyDetail,
    DesktopGenericBackLink,
  },
  data() {
    return {
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
