<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]">
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

    <generic-button id="back-button"
                    :button-classes="['grey', 'button']" :class="$style.back"
                    tabindex="0" @click.prevent="onBackButtonClicked">
      {{ $t('nominatedPharmacyCannotChange.backButton') }}
    </generic-button>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import { PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    PharmacySummary,
    PharmacyOpeningTimes,
  },
  data() {
    return {
      pharmacy: this.$store.state.nominatedPharmacy.pharmacy,
    };
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path, null);
    }
  },
  methods: {
    onBackButtonClicked() {
      redirectTo(this, PRESCRIPTIONS.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/info';
  @import '../../style/buttons';
  @import "../../style/home";

div {
  &.desktopWeb {
  max-width: 540px;

  li {
    font-family: $default_web;
    font-weight: normal;
  }

  p {
    font-family: $default_web;
    font-weight: normal;
    }
  }
}
</style>
