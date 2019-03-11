<template>
  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content']" >
    <p>{{ $t('nominatedPharmacy.line1') }}</p>
    <h2 :class="$style['pharmacy-name']"> {{ nominatedPharmacy.pharmacyName }} </h2>
    <p> {{ formatAddress(nominatedPharmacy) }} </p>
    <p> {{ nominatedPharmacy.telephoneNumber }} </p>
    <analytics-tracked-tag :click-func="goToChangeNominatedPharmacySearch"
                           :class="[$style.checkFeaturesLink, $style['link']]"
                           :text="$t('nominatedPharmacy.changePharmacyLink')"
                           tag="a"
                           tabindex="0">
      {{ $t('nominatedPharmacy.changePharmacyLink') }}
    </analytics-tracked-tag>
    <div :class="[$style.panel, $style['openingTime-panel']]">
      <h2 :class="$style['openingTime-label']">{{ $t('nominatedPharmacy.openingTimes') }}</h2>

      <div v-for="(openingTime, index) in nominatedPharmacy.openingTimes" :key="index">
        <div :class="$style['row']">
          <div :class="$style['column']">
            {{ openingTime.day }}
          </div>
          <div :class="$style['column']">
            {{ openingTime.time }}
          </div>
        </div>
      </div>
    </div>
    <analytics-tracked-tag :text="$t('th03.errors.backButton')">
      <generic-button
        :button-classes="['grey', 'button']" :class="$style.back"
        tabindex="0" @click.prevent="backButtonClicked">
        {{ $t('th03.errors.backButton') }}
      </generic-button>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import { PRESCRIPTIONS, NOMINATED_PHARMACY_SEARCH } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    GenericButton,
  },
  head() {
    return {
      title: `${this.getTitle} - ${this.$t('appTitle')}`,
    };
  },
  async asyncData({ store }) {
    if (store.state.nominatedPharmacy.hasLoaded === false) {
      await store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
    };
  },
  computed: {
  },
  mounted() {
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
    backButtonClicked() {
      redirectTo(this, PRESCRIPTIONS.path, null);
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

.openingTime-label {
    color: $nhs_blue !important;
    margin-bottom: 0.5em;
}

.link {
    margin-top: 0.5em;
    margin-bottom: 1.5em;
    cursor: pointer;
    text-decoration: underline;
}

.openingTime-panel {
  margin-bottom: 2em;
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
</style>
