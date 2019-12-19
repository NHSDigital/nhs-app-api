<template>
  <div>
    <div id="pharmacyName">
      <h3 class="nhsuk-u-padding-bottom-1"> {{ pharmacy.pharmacyName }} </h3>
    </div>
    <p v-if="!isInternetPharmacy" id="pharmacyAddress"> {{ formatAddress(pharmacy) }} </p>
    <analytics-tracked-tag v-if="isInternetPharmacy"
                           id="url"
                           :class="[$style.checkFeaturesLink, $style['link-spacing'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]"
                           :href="pharmacy.url"
                           :text="$t(pharmacy.url)"
                           tag="a" target="_blank">
      {{ pharmacy.url }}
    </analytics-tracked-tag>
    <p id="phoneNumber">
      {{ $t('nominated_pharmacy.changeSuccess.telephoneLabel') }}{{ pharmacy.telephoneNumber }}
    </p>
  </div>
</template>

<script>
import PharmacySubType from '@/lib/pharmacy-detail/pharmacy-sub-types';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'PharmacyChangeSuccessDetails',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    pharmacy: {
      type: Object,
      required: true,
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
