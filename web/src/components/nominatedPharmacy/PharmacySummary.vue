<template>
  <div>
    <div id="pharmacyName">
      <h2 v-if="pharmacyNameAsHeader" class="nhsuk-u-margin-bottom-1">
        {{ pharmacy.pharmacyName }} </h2>
      <p v-else> {{ pharmacy.pharmacyName }} </p>
    </div>
    <div>
      <p v-if="!isInternetPharmacy" id="pharmacyAddress" class="nhsuk-u-margin-bottom-1">
        {{ formatAddress(pharmacy) }}</p>
      <analytics-tracked-tag v-if="isInternetPharmacy"
                             id="url"
                             :href="pharmacy.url"
                             :text="$t(pharmacy.url)"
                             tag="a" target="_blank">
        {{ pharmacy.url }}
      </analytics-tracked-tag>
      <p id="phoneNumber" class="nhsuk-u-margin-bottom-1"> {{ pharmacy.telephoneNumber }} </p>
    </div>
  </div>
</template>

<script>
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'PharmacySummary',
  components: {
    AnalyticsTrackedTag,
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
  @import '../../style/listmenu';
  @import "../../style/panels";
  @import "../../style/colours";
  @import "../../style/textstyles";
  @import "../../style/home";

</style>
