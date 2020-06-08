<template>
  <menu-item id="btn_alreadyRegistered"
             header-tag="h3"
             :href="alreadyRegisteredUrl"
             target="_blank"
             :text="$t('organDonation.links.alreadyRegisteredText')"
             :aria-label="$t('organDonation.links.alreadyRegisteredText')"/>
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
      alreadyRegisteredUrl: this.$store.app.$env.ORGAN_DONATION_ALREADY_REGISTERED_URL,
    };
  },
};
</script>
