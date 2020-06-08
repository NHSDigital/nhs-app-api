<template>
  <menu-item id="btn_findOutMore"
             header-tag="h3"
             :href="findOutMoreUrl"
             target="_blank"
             :text="$t('organDonation.links.findOutMoreText')"
             :aria-label="$t('organDonation.links.findOutMoreText')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';

export default {
  name: 'FindOutMoreLink',
  components: {
    MenuItem,
  },
  data() {
    return {
      findOutMoreUrl: this.$store.app.$env.ORGAN_DONATION_FIND_OUT_MORE_URL,
    };
  },
};
</script>
