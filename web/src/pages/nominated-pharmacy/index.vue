<template>
  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content']" >
    <div v-if="hasNoNominatedPharmacy">
      <no-nominated-pharmacy-warning/>
    </div>
    <div v-else
         :class="$style.info" data-purpose="info">
      <pharmacy-detail
        :pharmacy="nominatedPharmacy"
        :is-my-nominated-pharmacy="true"
        :previous-path="currentPage"/>
    </div>
    <analytics-tracked-tag :text="$t('generic.backButton.text')">
      <generic-button
        :id="'back-button'"
        :button-classes="['grey', 'button']"
        :class="$style.back"
        tabindex="0"
        @click.prevent="backButtonClicked">
        {{ $t('generic.backButton.text') }}
      </generic-button>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import { PRESCRIPTIONS, NOMINATED_PHARMACY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    PharmacyDetail,
    NoNominatedPharmacyWarning,
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
      currentPage: NOMINATED_PHARMACY.path,
    };
  },
  async asyncData({ store }) {
    if (store.state.nominatedPharmacy.hasLoaded === false) {
      await store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  },
  methods: {
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
</style>
