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
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_CHOOSE_TYPE } from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
  },
  data() {
    return {
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
    };
  },
  created() {
    this.$store.dispatch('nominatedPharmacy/clearChosenType');
  },
  methods: {
    continueButtonClicked() {
      redirectTo(this, NOMINATED_PHARMACY_CHOOSE_TYPE.path);
    },
  },
};
</script>

<style scoped>

</style>
