<template>
  <div>
    <p v-if="pharmacy.addressLine1" id="pharmacy-address-line-1"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.addressLine1 }}</p>
    <p v-if="pharmacy.addressLine2" id="pharmacy-address-line-2"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.addressLine2 }}</p>
    <p v-if="pharmacy.addressLine3" id="pharmacy-address-line-3"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.addressLine3 }}</p>
    <p v-if="pharmacy.city" id="pharmacy-city"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.city }}</p>
    <p v-if="pharmacy.county" id="pharmacy-county"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.county }}</p>
    <p v-if="pharmacy.postcode" id="pharmacy-postcode" class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.postcode }}</p>
    <p v-if="isOnline && pharmacy.url" id="pharmacy-url"
       class="nhsuk-u-margin-bottom-3">
      {{ pharmacy.url }}</p>
    <p v-if="pharmacy.telephoneNumber" id="pharmacy-telephone-number"
       class="nhsuk-u-margin-bottom-3">
      {{ $t('nominated_pharmacy.telephoneLabel') +
        pharmacy.telephoneNumber }}</p>
    <p v-if="pharmacy.distance !== null" id="pharmacy-distance-away">
      {{ $t('nominatedPharmacySearchResults.distanceAway').
        replace('{distance}', pharmacy.distance) }}
    </p>
  </div>
</template>

<script>

import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';

export default {
  name: 'PharmacyAddressComponent',
  layout: 'nhsuk-layout',
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      isOnline: (this.$store.state.nominatedPharmacy.chosenType ===
        PharmacyTypeChoice.ONLINE_PHARMACY),
    };
  },
};
</script>

<style scoped>

</style>
