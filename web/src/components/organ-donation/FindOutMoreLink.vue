<template>
  <menu-item id="btn_findOutMore"
             header-tag="h3"
             :href="findOutMoreUrl"
             target="_blank"
             :text="$t('organDonation.findOutMoreAbout')"
             :aria-label="$t('organDonation.findOutMoreAbout')"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { ORGAN_DONATION_FIND_OUT_MORE_URL } from '@/router/externalLinks';

export default {
  name: 'FindOutMoreLink',
  components: {
    MenuItem,
  },
  data() {
    return {
      findOutMoreUrl: ORGAN_DONATION_FIND_OUT_MORE_URL,
    };
  },
};
</script>
