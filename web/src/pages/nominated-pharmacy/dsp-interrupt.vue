<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>
          {{ $t('nominated_pharmacy.dspInterrupt.paragraph') }}
        </p>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>
          <analytics-tracked-tag id="dsp-link"
                                 :text="$t('nominated_pharmacy.dspInterrupt.visitOnlineListText')"
                                 target="_blank"
                                 tag="a"
                                 :href="visitOnlinePharmacyListPath"
                                 style="vertical-align: baseline; display: inline;">
            {{ $t('nominated_pharmacy.dspInterrupt.visitOnlineListText') }}
          </analytics-tracked-tag>
        </p>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag
          :text="$t('nominated_pharmacy.dspInterrupt.returnToPrescriptionsText')">
          <desktopGenericBackLink
            id="prescriptions-home-link"
            :path="returnToPrescriptionsPath"
            :button-text="'nominated_pharmacy.dspInterrupt.returnToPrescriptionsText'"
            @clickAndPrevent="gotoPrescriptionsClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>

<script>
import { redirectTo } from '@/lib/utils';
import { PRESCRIPTIONS_PATH } from '@/router/paths';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import {
  NOMINATED_PHARMACY_DSP_URL,
} from '@/router/externalLinks';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      visitOnlinePharmacyListPath: NOMINATED_PHARMACY_DSP_URL,
      returnToPrescriptionsPath: PRESCRIPTIONS_PATH,
    };
  },
  methods: {
    gotoPrescriptionsClicked() {
      redirectTo(this, this.returnToPrescriptionsPath);
    },
  },
};
</script>

<style scoped>
</style>
