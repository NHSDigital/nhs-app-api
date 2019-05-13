<template>
  <div>
    <p>{{ $t('nominated_pharmacy.line1') }}</p>
    <p v-if="!isMyNominatedPharmacy">{{ $t('nominated_pharmacy.confirm.line1') }}</p>
    <pharmacy-summary id="pharmacy-summary"
                      :pharmacy="pharmacy" />
    <hr>
    <analytics-tracked-tag v-if="showChangeNominatedPharmacyLink"
                           id="link-to-change-pharmacy"
                           :click-func="goToChangeNominatedPharmacySearch"
                           :class="[$style['spacing-top']]"
                           :text="$t('nominated_pharmacy.changePharmacyLink')"
                           tag="a"
                           tabindex="0">
      {{ $t('nominated_pharmacy.changePharmacyLink') }}
    </analytics-tracked-tag>
    <pharmacy-opening-times id="pharmacy-opening-times"
                            :pharmacy-opening-time="pharmacy.openingTimesFormatted"
                            :class="[$style['spacing-top']]" />
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_CANNOT_CHANGE, PRESCRIPTIONS } from '@/lib/routes';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import { redirectTo } from '@/lib/utils';

export default {
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
  },
  computed: {
    showChangeNominatedPharmacyLink() {
      return (this.isMyNominatedPharmacy && this.canChangePharmacy);
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

.pharmacy-name {
  margin-top: 0.5em;
  margin-bottom: 0.2em;
}

.column {
  float: left;
  width: 50%;
}

.row:after {
  content: "";
  display: table;
  clear: both;
  padding-bottom: 0.5em;
}

.additional-padding {
  margin-top: 1em;
}

.spacing-top {
  margin: 1.5em 0;
}

</style>
