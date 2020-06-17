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
import { ORGAN_DONATION_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import MenuItem from '@/components/MenuItem';
import {
  ORGAN_DONATION_URL,
} from '@/router/externalLinks';

const getIsNativeApp = get('$store.state.device.isNativeApp');

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
      return this.useIntegratedOrganDonation ? ORGAN_DONATION_PATH : ORGAN_DONATION_URL;
    },
    useIntegratedOrganDonation() {
      // Integrated organ donation is used if the request is from the native app.
      return getIsNativeApp(this);
    },
  },
  methods: {
    onClickOrganDonation() {
      if (this.useIntegratedOrganDonation) {
        this.$store.dispatch('navigation/setBackLinkOverride', this.backLinkOverride);

        redirectTo(this, ORGAN_DONATION_PATH);
      }
    },
  },
};
</script>
