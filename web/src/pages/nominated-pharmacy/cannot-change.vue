<template>
  <div v-if="showTemplate"
       id="mainDiv">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div :class="$style.info" data-purpose="info">
          <p>{{ $t('nominatedPharmacyCannotChange.line1') }}</p>
          <h2>{{ $t('nominatedPharmacyCannotChange.howToHeader') }}</h2>
          <p>{{ $t('nominatedPharmacyCannotChange.line2') }}</p>
          <hr>
          <pharmacy-summary id="pharmacy-summary"
                            :pharmacy="pharmacy" />
          <pharmacy-opening-times id="pharmacy-opening-times"
                                  :pharmacy-opening-time="pharmacy.openingTimesFormatted" />
        </div>

        <generic-button v-if="$store.state.device.isNativeApp" id="back-button"
                        :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                        tabindex="0" @click.prevent="onBackButtonClicked">
          {{ $t('nominatedPharmacyCannotChange.backButton') }}
        </generic-button>
        <desktopGenericBackLink v-else id="back-button"
                                :path="prescriptions"
                                :button-text="'nominatedPharmacyCannotChange.backButton'"
                                @clickAndPrevent="onBackButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import { PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    PharmacySummary,
    PharmacyOpeningTimes,
    DesktopGenericBackLink,
  },
  data() {
    return {
      pharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      prescriptions: PRESCRIPTIONS.path,
    };
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, this.prescriptions);
    }
  },
  methods: {
    onBackButtonClicked() {
      redirectTo(this, this.prescriptions);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/info';
  @import '../../style/buttons';
  @import "../../style/home";
</style>
