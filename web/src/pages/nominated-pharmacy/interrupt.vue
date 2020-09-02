<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="!hasNoNominatedPharmacy" id="nominated-pharmacy-prescriptions-warning">
          <p id="prescriptions-warning-line-one">
            {{ $t('nominatedPharmacy.interrupt.nominatedPharmacyFoundLine1') }}</p>
          <p id="prescriptions-warning-line-two">
            {{ $t('nominatedPharmacy.interrupt.nominatedPharmacyFoundLine2') }}</p>
        </div>
        <generic-button id="continue-button"
                        :button-classes="['nhsuk-button', 'nhsuk-button--primary']"
                        @click.stop.prevent="continueButtonClicked">
          {{ $t('nominatedPharmacy.interrupt.continueButton') }}
        </generic-button>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.back')">
          <desktopGenericBackLink id="back-link"
                                  :path="previousPagePath"
                                  :button-text="'generic.back'"
                                  @clickAndPrevent="backButtonClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';
import {
  NOMINATED_PHARMACY_CHOOSE_TYPE_PATH,
  NOMINATED_PHARMACY_PATH,
  PRESCRIPTIONS_PATH,
  NOMINATED_PHARMACY_CHECK_PATH,
} from '@/router/paths';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import InterruptBackTo from '@/lib/pharmacy-detail/interrupt-back-to';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    DesktopGenericBackLink,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
    };
  },
  computed: {
    previousPagePath() {
      const backTo = this.$store.getters['nominatedPharmacy/getInterruptBackTo'];

      switch (backTo) {
        case InterruptBackTo.PRESCRIPTIONS:
          return PRESCRIPTIONS_PATH;
        case InterruptBackTo.NOMINATED_PHARMACY_SUMMARY:
          return NOMINATED_PHARMACY_PATH;
        case InterruptBackTo.NOMINATED_PHARMACY_CHECK:
          return NOMINATED_PHARMACY_CHECK_PATH;
        default:
          return PRESCRIPTIONS_PATH;
      }
    },
  },
  created() {
    this.$store.dispatch('nominatedPharmacy/clearSearchJourney');

    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS_PATH);
    }
  },
  methods: {
    continueButtonClicked() {
      redirectTo(this, NOMINATED_PHARMACY_CHOOSE_TYPE_PATH);
    },
    backButtonClicked() {
      redirectTo(this, this.previousPagePath);
    },
  },
};
</script>

<style scoped>

</style>
