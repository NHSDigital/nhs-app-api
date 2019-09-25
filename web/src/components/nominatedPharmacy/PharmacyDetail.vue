<template>
  <div>
    <p v-if="showInstruction" id="instruction">{{ instructionText }}</p>
    <pharmacy-summary id="pharmacy-summary"
                      :pharmacy="pharmacy" />
    <p v-if="isInternetPharmacy" id="statement"
       :class="[$style['spacing-top']]">{{ $t('nominated_pharmacy.internetPharmacy') }}</p>
    <pharmacy-opening-times v-if="!isInternetPharmacy" id="pharmacy-opening-times"
                            :pharmacy-opening-time="pharmacy.openingTimesFormatted" />
    <analytics-tracked-tag v-if="showChangeNominatedPharmacyLink"
                           id="link-to-change-pharmacy"
                           :click-func="goToChangeNominatedPharmacySearch"
                           :class="[$style.checkFeaturesLink, $style['link-spacing'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]"
                           :text="$t('nominated_pharmacy.changePharmacyLink')"
                           tag="a">
      {{ $t('nominated_pharmacy.changePharmacyLink') }}
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_CANNOT_CHANGE, PRESCRIPTIONS } from '@/lib/routes';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'PharmacyDetail',
  components: {
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
    goToChangeNominatedPharmacySearch() {
      this.$store.dispatch('nominatedPharmacy/setPreviousPageToSearch', this.previousPath);
      const nextPage = (this.pharmacy.pharmacyType === PharmacyType.P3) ?
        NOMINATED_PHARMACY_CANNOT_CHANGE.path : NOMINATED_PHARMACY_SEARCH.path;
      redirectTo(this, nextPage, null);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import "../../style/panels";
@import "../../style/colours";
@import "../../style/textstyles";
@import "../../style/home";

.row:after {
  content: "";
  display: table;
  clear: both;
  padding-bottom: 0.5em;
}

.checkFeaturesLink.link-spacing {
  display: inline-block;
  margin: 1.2em 0;
}

</style>
