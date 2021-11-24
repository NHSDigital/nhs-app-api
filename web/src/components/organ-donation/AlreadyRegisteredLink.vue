<template>
  <menu-item id="btn_alreadyRegistered"
             header-tag="h2"
             :href="alreadyRegisteredUrl"
             target="_blank"
             :text="$t('organDonation.thinkYouHaveRegisteredAlready')"
             :aria-label="$t('organDonation.thinkYouHaveRegisteredAlready')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';

export default {
  name: 'AlreadyRegisteredLink',
  components: {
    MenuItem,
  },
  data() {
    return {
      alreadyRegisteredUrl: this.$store.$env.ORGAN_DONATION_ALREADY_REGISTERED_URL,
    };
  },
};
</script>
