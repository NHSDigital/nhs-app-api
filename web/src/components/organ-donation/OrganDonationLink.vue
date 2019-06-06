<template>
  <analytics-tracked-tag :href="organDonationUrl"
                         :text="$t('sc04.organDonation.subheader')"
                         :tabindex="-1">
    <a :id="id"
       :class="className"
       :href="organDonationUrl"
       :target="organDonationTarget"
       @click="onClickOrganDonation($event)">
      <slot/>
    </a>
  </analytics-tracked-tag>
</template>

<script>
import get from 'lodash/fp/get';
import flow from 'lodash/fp/flow';

// For some reason, in this file, when the JEST tests run, it fails unless I add the '.vue'
// extension.  Other tests seem fine but this one fails!
// eslint-disable-next-line import/extensions
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag.vue';
import { ORGAN_DONATION } from '@/lib/routes';
import { isTruthy, redirectTo } from '@/lib/utils';

const getIsNativeApp = get('$store.state.device.isNativeApp');
const getOrganDonationUrl = get('$store.app.$env.ORGAN_DONATION_URL');
const getOrganDonationIntegrationEnabled = flow(
  get('$store.app.$env.ORGAN_DONATION_INTEGRATION_ENABLED'),
  isTruthy,
);

export default {
  name: 'OrganDonationLink',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    className: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    organDonationTarget() {
      return this.useIntegratedOrganDonation ? '_self' : '_blank';
    },

    organDonationUrl() {
      return this.useIntegratedOrganDonation ? ORGAN_DONATION.path : getOrganDonationUrl(this);
    },

    useIntegratedOrganDonation() {
      // Integrated organ donation is used if it has been switched on in the environment variables
      // and the request is from the native app.
      return getOrganDonationIntegrationEnabled(this) && getIsNativeApp(this);
    },
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname, null);
      event.preventDefault();
    },

    onClickOrganDonation(event) {
      if (this.useIntegratedOrganDonation) this.navigate(event);
    },
  },
};
</script>

<style module lang="scss">
</style>
