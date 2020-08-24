<template>
  <div>
    <div>
      <p id="pharmacyName" class="nhsuk-u-margin-bottom-0"> {{ pharmacy.pharmacyName }} </p>
    </div>
    <pharmacy-address-component :pharmacy="pharmacy"/>
    <h2>{{ $t('nominatedPharmacy.changeSuccess.whatHappensNext') }}</h2>
    <p>{{ $t('nominatedPharmacy.changeSuccess.highStreetPrescription').
      replace('{pharmacyName}', pharmacy.pharmacyName) }}</p>
    <p>{{ $t('nominatedPharmacy.changeSuccess.highStreetPrescriptionCollectionMessage').
      replace('{pharmacyName}', pharmacy.pharmacyName) }}</p>
  </div>
</template>

<script>
import PharmacyAddressComponent from './PharmacyAddressComponent';

export default {
  name: 'PharmacyChangeSuccessDetails',
  components: { PharmacyAddressComponent },
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
  },
};
</script>
