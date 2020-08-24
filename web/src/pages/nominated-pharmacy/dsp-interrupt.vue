<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>
          {{ $t('nominatedPharmacy.dspInterrupt.paragraph') }}
        </p>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>
          <analytics-tracked-tag id="dsp-link"
                                 :text="$t('nominatedPharmacy.dspInterrupt.visitOnlineListText')"
                                 target="_blank"
                                 tag="a"
                                 :href="visitOnlinePharmacyListPath"
                                 style="vertical-align: baseline; display: inline;">
            {{ $t('nominatedPharmacy.dspInterrupt.visitOnlineListText') }}
          </analytics-tracked-tag>
        </p>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag
          :text="$t('nominatedPharmacy.dspInterrupt.returnToPrescriptionsText')">
          <desktopGenericBackLink
            id="prescriptions-home-link"
            :path="returnToPrescriptionsPath"
            :button-text="'nominatedPharmacy.dspInterrupt.returnToPrescriptionsText'"
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
