<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <pharmacy-detail
      :nominated-pharmacy="nominatedPharmacy"
      :is-my-nominated-pharmacy="false" />
    <generic-button id="confirm-button"
                    :class="[$style.button, $style.green]"
                    @click.stop.prevent="submitNominatedPharmacy">
      {{ $t('confirmNominatedPharmacy.confirmButton') }}
    </generic-button>
    <analytics-tracked-tag :text="$t('th03.errors.backButton')">
      <generic-button
        :button-classes="['grey', 'button']" :class="$style['back']"
        tabindex="0" @click.prevent="cancelButtonClicked">
        {{ $t('th03.errors.backButton') }}
      </generic-button>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY } from '@/lib/routes';

export default {
  components: {
    GenericButton,
    AnalyticsTrackedTag,
    PharmacyDetail,
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.selectedNominatedPharmacy,
    };
  },
  created() {
    if (this.nominatedPharmacy === null) {
      redirectTo(this, NOMINATED_PHARMACY.path, null);
    }
  },
  methods: {
    async submitNominatedPharmacy() {
      await this.$store.dispatch('nominatedPharmacy/update', this.nominatedPharmacy.odsCode);
      this.$store.dispatch('flashMessage/addSuccess', this.$t('confirmNominatedPharmacy.pharmacyChanged'));
      this.$store.dispatch('nominatedPharmacy/clearSelectedNominatedPharmacy');
      redirectTo(this, NOMINATED_PHARMACY.path, null);
    },
    cancelButtonClicked() {
      redirectTo(this, NOMINATED_PHARMACY.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";
</style>
