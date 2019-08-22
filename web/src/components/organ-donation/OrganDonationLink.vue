<template>
  <menu-item :id="id"
             :href="organDonationUrl"
             :text="$t('sc04.organDonation.subheader')"
             :description="description"
             :header-tag="headerTag"
             :click-func="onClickOrganDonation"
             :prevent-default="useIntegratedOrganDonation"
             :target="organDonationTarget"
             :aria-label="ariaLabelCaption(
               'sc04.organDonation.subheader',
               description)"/>
</template>

<script>
import get from 'lodash/fp/get';
import flow from 'lodash/fp/flow';

// For some reason, in this file, when the JEST tests run, it fails unless I add the '.vue'
// extension.  Other tests seem fine but this one fails!
// eslint-disable-next-line import/extensions
import { ORGAN_DONATION } from '@/lib/routes';
import { isTruthy, redirectTo } from '@/lib/utils';
import MenuItem from '@/components/MenuItem';

const getIsNativeApp = get('$store.state.device.isNativeApp');
const getOrganDonationUrl = get('$store.app.$env.ORGAN_DONATION_URL');
const getOrganDonationIntegrationEnabled = flow(
  get('$store.app.$env.ORGAN_DONATION_INTEGRATION_ENABLED'),
  isTruthy,
);

export default {
  name: 'OrganDonationLink',
  components: {
    MenuItem,
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
    description: {
      type: String,
      default: undefined,
    },
    headerTag: {
      type: String,
      default: 'span',
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
      if (this.useIntegratedOrganDonation) {
        this.navigate(event);
      }
    },
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>

<style module lang="scss">
</style>
