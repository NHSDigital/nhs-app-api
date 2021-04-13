<template>
  <div>
    <p v-if="pharmacy.addressLine1" :id="'pharmacy-' + identifier + '-address-line-1'"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.addressLine1 }}</p>
    <p v-if="pharmacy.addressLine2" :id="'pharmacy-' + identifier + '-address-line-2'"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.addressLine2 }}</p>
    <p v-if="pharmacy.addressLine3" :id="'pharmacy-' + identifier + '-address-line-3'"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.addressLine3 }}</p>
    <p v-if="pharmacy.city" :id="'pharmacy-' + identifier + '-city'"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.city }}</p>
    <p v-if="pharmacy.county" :id="'pharmacy-' + identifier + '-county'"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.county }}</p>
    <p v-if="pharmacy.postcode" :id="'pharmacy-' + identifier + '-postcode'"
       class="nhsuk-u-margin-bottom-0">
      {{ pharmacy.postcode }}</p>
    <p v-if="isOnline && pharmacy.url" :id="'pharmacy-' + identifier + '-url'"
       class="nhsuk-u-margin-bottom-3">
      {{ displayUrl }}</p>
    <p v-if="pharmacy.telephoneNumber" :id="'pharmacy-' + identifier + '-telephone-number'"
       class="nhsuk-u-margin-bottom-3">
      {{ pharmacy.telephoneNumber }}</p>
    <p v-if="pharmacy.distance !== null" :id="'pharmacy-' + identifier + '-distance-away'">
      {{ $t('nominatedPharmacy.searchResults.distanceAway').
        replace('{distance}', pharmacy.distance) }}
    </p>
  </div>
</template>

<script>

import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import { displayedURL } from '@/lib/utils';

export default {
  name: 'PharmacyAddressComponent',
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
    identifier: {
      type: Number,
      required: false,
      default: 0,
    },
  },
  data() {
    return {
      isOnline: (this.$store.state.nominatedPharmacy.chosenType ===
        PharmacyTypeChoice.ONLINE_PHARMACY),
    };
  },
  computed: {
    displayUrl() {
      return displayedURL(this.pharmacy.url);
    },
  },
};
</script>
