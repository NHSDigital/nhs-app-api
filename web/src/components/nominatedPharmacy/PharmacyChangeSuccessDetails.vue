<template>
  <div>
    <div>
      <p id="pharmacyName" class="nhsuk-u-margin-bottom-0"> {{ pharmacy.pharmacyName }} </p>
    </div>
    <pharmacy-address-component :pharmacy="pharmacy"/>
    <h2>{{ $t('nominated_pharmacy.changeSuccess.whatHappensNext') }}</h2>
    <p>{{ $t('nominated_pharmacy.changeSuccess.highStreetPrescription').
      replace('{pharmacyName}', pharmacy.pharmacyName) }}</p>
    <p>{{ $t('nominated_pharmacy.changeSuccess.highStreetPrescriptionCollectionMessage').
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
