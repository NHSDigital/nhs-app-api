<template>
  <div>
    <p>{{ $t('nominatedPharmacy.line1') }}</p>
    <p v-if="!isMyNominatedPharmacy">{{ $t('confirmNominatedPharmacy.line1') }}</p>
    <h2 :class="$style['pharmacy-name']"> {{ nominatedPharmacy.pharmacyName }} </h2>
    <p> {{ formatAddress(nominatedPharmacy) }} </p>
    <p> {{ nominatedPharmacy.telephoneNumber }} </p>
    <analytics-tracked-tag v-if="isMyNominatedPharmacy"
                           :click-func="goToChangeNominatedPharmacySearch"
                           :class="[$style.checkFeaturesLink, $style['link']]"
                           :text="$t('nominatedPharmacy.changePharmacyLink')"
                           tag="a"
                           tabindex="0">
      {{ $t('nominatedPharmacy.changePharmacyLink') }}
    </analytics-tracked-tag>

    <collapsible-dialog :class="$style['opening-times']">
      <template slot="header">
        {{ $t('nominatedPharmacy.openingTimes') }}
      </template>
      <div>
        <div v-for="(openingTimeDetail, i)
               in nominatedPharmacy.openingTimesFormatted"
             :key="i">
          <div :class="$style['row']">
            <div :class="$style['column']">{{ openingTimeDetail.day }}</div>
            <div :class="$style['column']">
              <div
                v-if="openingTimeDetail.times.length === 0">{{ $t('nominatedPharmacy.closed') }}
              </div>
              <div v-else>
                <div v-for="(openingTime, j) in openingTimeDetail.times"
                     :key="j">{{ openingTime }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </collapsible-dialog>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CollapsibleDialog from '@/components/widgets/CollapsibleDialog';
import { NOMINATED_PHARMACY_SEARCH } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    CollapsibleDialog,
  },
  props: {
    nominatedPharmacy: {
      type: Object,
      required: true,
    },
    isMyNominatedPharmacy: {
      type: Boolean,
      required: true,
    },
  },
  methods: {
    formatAddress(nominatedPharmacy) {
      return [
        nominatedPharmacy.addressLine1,
        nominatedPharmacy.addressLine2,
        nominatedPharmacy.addressLine3,
        nominatedPharmacy.county,
        nominatedPharmacy.city,
        nominatedPharmacy.postcode,
      ].filter(Boolean).join(', ');
    },
    goToChangeNominatedPharmacySearch() {
      redirectTo(this, NOMINATED_PHARMACY_SEARCH.path, null);
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

.opening-times {
  margin-bottom: 1em;
  margin-top: 1em;
}

.link {
    margin-top: 0.5em;
    cursor: pointer;
    text-decoration: underline;
}

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
</style>
