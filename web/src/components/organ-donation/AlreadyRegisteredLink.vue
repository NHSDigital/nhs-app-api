<template>
  <menu-item id="btn_alreadyRegistered"
             header-tag="h3"
             :href="alreadyRegisteredUrl"
             target="_blank"
             :text="$t('organDonation.thinkYouHaveRegisteredAlready')"
             :aria-label="$t('organDonation.thinkYouHaveRegisteredAlready')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { ORGAN_DONATION_ALREADY_REGISTERED_URL } from '@/router/externalLinks';

export default {
  name: 'AlreadyRegisteredLink',
  components: {
    MenuItem,
  },
  data() {
    return {
      alreadyRegisteredUrl: ORGAN_DONATION_ALREADY_REGISTERED_URL,
    };
  },
};
</script>
