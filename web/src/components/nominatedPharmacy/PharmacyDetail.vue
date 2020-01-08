<template>
  <div>
    <div class="nhsuk-body">
      <p v-if="showInstruction" id="instruction" class="nhsuk-u-margin-top-0">
        {{ instructionText }}</p>
      <pharmacy-summary id="pharmacy-summary"
                        :pharmacy="pharmacy" />
      <p v-if="isInternetPharmacy" id="statement"
         :class="[$style['spacing-top']]">{{ $t('nominated_pharmacy.internetPharmacy') }}</p>
      <pharmacy-opening-times v-if="!isInternetPharmacy" id="pharmacy-opening-times"
                              :pharmacy-opening-time="pharmacy.openingTimesFormatted" />
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
    <analytics-tracked-tag v-if="showChangeNominatedPharmacyLink &&
                             !displayChangeMyNominatedPharmacyButton"
                           id="link-to-change-pharmacy"
                           :text="$t('nominated_pharmacy.changePharmacyLink')"
                           :click-func="goToChangeNominatedPharmacySearch"
                           tag="a" :href="nominatedPharmacySearch">
      <p> {{ $t('nominated_pharmacy.changePharmacyLink') }} </p>
    </analytics-tracked-tag>
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

export default {
  name: 'PharmacyDetail',
  components: {
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
      nominatedPharmacySearch: NOMINATED_PHARMACY_SEARCH.path,
    };
  },
  computed: {
    showChangeNominatedPharmacyLink() {
      return (this.isMyNominatedPharmacy && this.canChangePharmacy);
    },
    isInternetPharmacy() {
      if (this.pharmacy.pharmacySubType === PharmacySubType.InternetPharmacy) {
        return true;
      }
      return false;
    },
    instructionText() {
      return this.$t('nominated_pharmacy.confirm.line1');
    },
  },
  methods: {
    goToNominatedPharmacyInterruptPage() {
      this.$store.dispatch('nominatedPharmacy/setPreviousPageToSearch', this.previousPath);
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
