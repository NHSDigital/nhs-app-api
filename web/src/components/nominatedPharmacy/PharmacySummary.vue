<template>
  <div>
    <div id="pharmacyName" :class="$style['pharmacy-name']">
      <h2 v-if="pharmacyNameAsHeader"> {{ pharmacy.pharmacyName }} </h2>
      <p v-else> {{ pharmacy.pharmacyName }} </p>
    </div>
    <p v-if="!isInternetPharmacy" id="pharmacyAddress"> {{ formatAddress(pharmacy) }} </p>
    <analytics-tracked-tag v-if="isInternetPharmacy"
                           id="url"
                           :href="pharmacy.url"
                           :text="$t(pharmacy.url)"
                           tag="a" target="_blank">
      {{ pharmacy.url }}
    </analytics-tracked-tag>
    <p id="phoneNumber"> {{ pharmacy.telephoneNumber }} </p>
  </div>
</template>

<script>
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
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
        pharmacy.county,
        pharmacy.city,
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

.pharmacy-name {
  margin-top: 0.5em;
  margin-bottom: 0.2em;
}

.additional-padding {
  margin-top: 1em;
}
</style>
