<template>
  <menu-item :id="id"
             :href="organDonationUrl"
             :text="$t('sc04.organDonation.subheader')"
             :description="linkDescription"
             :header-tag="headerTag"
             :click-func="onClickOrganDonation"
             :prevent-default="useIntegratedOrganDonation"
             :target="organDonationTarget"
             :aria-label="$t('sc04.organDonation.subheader') | join(linkDescription ,'. ')"/>
</template>

<script>
import get from 'lodash/fp/get';

// For some reason, in this file, when the JEST tests run, it fails unless I add the '.vue'
// extension.  Other tests seem fine but this one fails!
// eslint-disable-next-line import/extensions
import { ORGAN_DONATION } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import MenuItem from '@/components/MenuItem';

const getIsNativeApp = get('$store.state.device.isNativeApp');
const getOrganDonationUrl = get('$store.app.$env.ORGAN_DONATION_URL');

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
    displayDescription: {
      type: Boolean,
      default: false,
    },
    headerTag: {
      type: String,
      default: 'h2',
    },
    backLinkOverride: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    linkDescription() {
      if (this.displayDescription) {
        return this.$t('sc04.organDonation.body');
      }
      return '';
    },
    organDonationTarget() {
      return this.useIntegratedOrganDonation ? '_self' : '_blank';
    },
    organDonationUrl() {
      return this.useIntegratedOrganDonation ? ORGAN_DONATION.path : getOrganDonationUrl(this);
    },
    useIntegratedOrganDonation() {
      // Integrated organ donation is used if the request is from the native app.
      return getIsNativeApp(this);
    },
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();
    },
    onClickOrganDonation(event) {
      if (this.useIntegratedOrganDonation) {
        this.$store.dispatch('navigation/setBackLinkOverride', this.backLinkOverride);

        this.navigate(event);
      }
    },
  },
};
</script>
