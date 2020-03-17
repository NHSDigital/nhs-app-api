<template>
  <div>
    <div class="nhsuk-body">
      <p v-if="showInstruction" id="instruction" class="nhsuk-u-margin-top-0">
        {{ instructionText }}</p>

      <div v-if="isHighStreetPharmacy">
        <pharmacy-summary id="pharmacy-summary"
                          :pharmacy="pharmacy" />
        <pharmacy-opening-times v-if="!isInternetPharmacy" id="pharmacy-opening-times"
                                :pharmacy-opening-time="pharmacy.openingTimesFormatted" />
      </div>
      <div v-else-if="isInternetPharmacy"
           id="internet-pharmacy-div" class="nhsuk-u-padding-bottom-5">
        <online-only-pharmacy-detail id="online-pharmacy-summary" :pharmacy="pharmacy"/>
      </div>
      <analytics-tracked-tag v-if="showChangeNominatedPharmacyLink &&
                               displayChangeMyNominatedPharmacyButton"
                             id="button-to-change-pharmacy"
                             :click-func="goToNominatedPharmacyInterruptPage"
                             :text="$t('nominated_pharmacy.changePharmacyLink')">
        <generic-button class="nhsuk-button">
          {{ $t('nominated_pharmacy.changePharmacyLink') }}
        </generic-button>
      </analytics-tracked-tag>
    </div>
    <p v-if="showChangeNominatedPharmacyLink && !displayChangeMyNominatedPharmacyButton">
      <analytics-tracked-tag id="link-to-change-pharmacy"
                             :text="$t('nominated_pharmacy.changePharmacyLink')"
                             :click-func="goToNominatedPharmacyInterruptPage"
                             tag="a" :href="nominatedPharmacyInterrupt"
                             style="vertical-align: baseline; display: inline;">
        {{ $t('nominated_pharmacy.changePharmacyLink') }}
      </analytics-tracked-tag>
    </p>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { PRESCRIPTIONS, NOMINATED_PHARMACY_INTERRUPT } from '@/lib/routes';
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import { redirectTo } from '@/lib/utils';
import GenericButton from '@/components/widgets/GenericButton';
import OnlineOnlyPharmacyDetail from './OnlineOnlyPharmacyDetail';

export default {
  name: 'PharmacyDetail',
  components: {
    OnlineOnlyPharmacyDetail,
    GenericButton,
    AnalyticsTrackedTag,
    PharmacySummary,
    PharmacyOpeningTimes,
  },
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
    isMyNominatedPharmacy: {
      type: Boolean,
      required: true,
    },
    previousPath: {
      type: String,
      required: false,
      default: PRESCRIPTIONS.path,
    },
    canChangePharmacy: {
      type: Boolean,
      required: false,
    },
    showInstruction: {
      type: Boolean,
      required: false,
      default: true,
    },
    displayChangeMyNominatedPharmacyButton: {
      type: Boolean,
      required: false,
      default: true,
    },
  },
  data() {
    return {
      nominatedPharmacyInterrupt: NOMINATED_PHARMACY_INTERRUPT.path,
    };
  },
  computed: {
    showChangeNominatedPharmacyLink() {
      return (this.isMyNominatedPharmacy && this.canChangePharmacy);
    },
    isInternetPharmacy() {
      return this.pharmacy.pharmacySubType === PharmacySubType.InternetPharmacy;
    },
    isHighStreetPharmacy() {
      return this.pharmacy.pharmacySubType === PharmacySubType.CommunityPharmacy;
    },
    instructionText() {
      return this.$t('nominated_pharmacy.confirm.line1');
    },
  },
  methods: {
    goToNominatedPharmacyInterruptPage() {
      redirectTo(this, NOMINATED_PHARMACY_INTERRUPT.path);
    },
  },
};

</script>

<style module lang="scss" scoped>

  .nhsuk-back-link__link {
    display: inline-block;
    cursor: pointer;
  }

</style>
