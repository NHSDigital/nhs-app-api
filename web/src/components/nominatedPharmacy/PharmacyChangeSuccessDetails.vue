<template>
  <div>
    <div id="pharmacyName">
      <p class="nhsuk-u-margin-bottom-0"> {{ pharmacy.pharmacyName }} </p>
    </div>
    <p id="pharmacyAddress" class="nhsuk-u-margin-bottom-0"> {{ formatAddress(pharmacy) }} </p>
    <p id="phoneNumber">
      {{ $t('nominated_pharmacy.changeSuccess.telephoneLabel') }}{{ pharmacy.telephoneNumber }}
    </p>
    <h2>{{ $t('nominated_pharmacy.changeSuccess.whatHappensNext') }}</h2>
    <p>{{ $t('nominated_pharmacy.changeSuccess.highStreetPrescription').
      replace('{pharmacyName}', pharmacy.pharmacyName) }}</p>
    <p>{{ $t('nominated_pharmacy.changeSuccess.highStreetPrescriptionCollectionMessage').
      replace('{pharmacyName}', pharmacy.pharmacyName) }}</p>
  </div>
</template>

<script>

export default {
  name: 'PharmacyChangeSuccessDetails',
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
  },
  methods: {
    formatAddress(pharmacy) {
      return [
        pharmacy.addressLine1,
        pharmacy.addressLine2,
        pharmacy.addressLine3,
        pharmacy.city,
        pharmacy.county,
        pharmacy.postcode,
      ].filter(Boolean).join(', ');
    },
  },
};
</script>
