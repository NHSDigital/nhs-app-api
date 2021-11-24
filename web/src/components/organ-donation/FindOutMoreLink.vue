<template>
  <menu-item id="btn_findOutMore"
             header-tag="h2"
             :href="findOutMoreUrl"
             target="_blank"
             :text="$t('organDonation.findOutMoreAbout')"
             :aria-label="$t('organDonation.findOutMoreAbout')"/>
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
      findOutMoreUrl: this.$store.$env.ORGAN_DONATION_FIND_OUT_MORE_URL,
    };
  },
};
</script>
