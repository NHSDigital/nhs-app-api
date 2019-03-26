<template>
  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content']" >
    <pharmacy-detail
      :nominated-pharmacy="nominatedPharmacy"
      :is-my-nominated-pharmacy="true" />
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
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import { PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    PharmacyDetail,
  },
  head() {
    return {
      title: `${this.getTitle} - ${this.$t('appTitle')}`,
    };
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
    };
  },
  async asyncData({ store }) {
    if (store.state.nominatedPharmacy.hasLoaded === false) {
      await store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  },
  mounted() {
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
