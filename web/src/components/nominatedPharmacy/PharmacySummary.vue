<template>
  <div>
    <div>
      <p id="pharmacyName" class="nhsuk-u-margin-bottom-0"> {{ pharmacy.pharmacyName }} </p>
      <p v-if="!isInternetPharmacy" id="pharmacyAddress" class="nhsuk-u-margin-bottom-1">
        <pharmacy-address-component id="pharmacy-address-component" :pharmacy="pharmacy"/>
      </p>
    </div>
  </div>
</template>

<script>
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import PharmacyAddressComponent from './PharmacyAddressComponent';

export default {
  name: 'PharmacySummary',
  components: {
    PharmacyAddressComponent,
  },
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
    pharmacyNameAsHeader: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    isInternetPharmacy() {
      return (this.pharmacy.pharmacySubType === PharmacySubType.InternetPharmacy);
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

<style module lang="scss" scoped>
  @import "@/style/_listmenu";
  @import "@/style/_panels";
  @import "@/style/_colours";
  @import "@/style/_textstyles";
  @import "@/style/_home";
</style>
