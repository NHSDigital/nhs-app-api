<template>
  <div>
    <div class="nhsuk-body">
      <p id="pharmacyName" class="nhsuk-u-margin-bottom-0">{{ pharmacy.pharmacyName }}</p>
      <analytics-tracked-tag v-if="pharmacy.url"
                             :href="hrefForUrl"
                             :text="displayUrl"
                             tag="a" target="_blank">
        <p id="url" class="nhsuk-u-margin-bottom-0">{{ displayUrl }}</p>
      </analytics-tracked-tag>
      <p v-if="pharmacy.telephoneNumber" id="online-only-phone-number">
        {{ $t('nominated_pharmacy.confirm.telephoneLabel') +
          pharmacy.telephoneNumber }}</p>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { NOMINATED_PHARMACY_INTERRUPT } from '@/lib/routes';
import { hrefForURL, displayedURL } from '@/lib/utils';

export default {
  name: 'OnlineOnlyPharmacyDetail',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      nominatedPharmacyInterrupt: NOMINATED_PHARMACY_INTERRUPT.path,
    };
  },
  computed: {
    instructionText() {
      return this.$t('nominated_pharmacy.confirm.line1');
    },
    displayUrl() {
      return displayedURL(this.pharmacy.url);
    },
    hrefForUrl() {
      return hrefForURL(this.pharmacy.url);
    },
  },
};

</script>
