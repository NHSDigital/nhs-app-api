<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="!hasNoNominatedPharmacy" id="nominated-pharmacy-prescriptions-warning">
          <p id="prescriptions-warning-line-one">
            {{ $t('nominated_pharmacy.interrupt.nominatedPharmacyFoundLine1') }}</p>
          <p id="prescriptions-warning-line-two">
            {{ $t('nominated_pharmacy.interrupt.nominatedPharmacyFoundLine2') }}</p>
        </div>
        <generic-button id="continue-button"
                        :button-classes="['nhsuk-button', 'nhsuk-button--primary']"
                        @click.stop.prevent="continueButtonClicked">
          {{ $t('nominated_pharmacy.interrupt.continueButton') }}
        </generic-button>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink id="back-link"
                                  :path="previousPagePath"
                                  :button-text="'generic.backButton.text'"
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
  NOMINATED_PHARMACY_CHOOSE_TYPE,
  NOMINATED_PHARMACY, PRESCRIPTIONS,
  NOMINATED_PHARMACY_CHECK,
} from '@/lib/routes';
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
          return PRESCRIPTIONS.path;
        case InterruptBackTo.NOMINATED_PHARMACY_SUMMARY:
          return NOMINATED_PHARMACY.path;
        case InterruptBackTo.NOMINATED_PHARMACY_CHECK:
          return NOMINATED_PHARMACY_CHECK.path;
        default:
          return PRESCRIPTIONS.path;
      }
    },
  },
  created() {
    this.$store.dispatch('nominatedPharmacy/clearSearchJourney');

    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path);
    }
  },
  methods: {
    continueButtonClicked() {
      redirectTo(this, NOMINATED_PHARMACY_CHOOSE_TYPE.path);
    },
    backButtonClicked() {
      redirectTo(this, this.previousPagePath);
    },
  },
};
</script>

<style scoped>

</style>
