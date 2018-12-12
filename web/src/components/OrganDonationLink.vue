<template>
  <analytics-tracked-tag>
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
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { ORGAN_DONATION } from '@/lib/routes';
import { isTruthy } from '@/lib/utils';

const getIsNativeApp = get('$store.state.device.isNativeApp');
const getOrganDonationUrl = get('$store.app.$env.ORGAN_DONATION_URL');
const getOrganDonationIntegrationEnabled = flow(
  get('$store.app.$env.ORGAN_DONATION_INTEGRATION_ENABLED'),
  isTruthy,
);

export default {
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    className: {
      type: String,
      default: '',
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
      this.$router.push(event.currentTarget.pathname);
      event.preventDefault();
    },

    onClickOrganDonation(event) {
      if (this.useIntegratedOrganDonation) this.navigate(event);
    },
  },
};
</script>

<style module lang="scss">
  .no-decoration {
    text-decoration: none;
  }
</style>
