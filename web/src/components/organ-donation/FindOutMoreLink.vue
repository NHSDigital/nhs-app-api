<template>
  <analytics-tracked-tag :href="findOutMoreUrl" :text="$t('organDonation.links.findOutMoreText')"
                         tag="a" target="_blank">
    {{ $t('organDonation.links.findOutMoreText') }}
  </analytics-tracked-tag>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'FindOutMoreLink',
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      findOutMoreUrl: this.$store.app.$env.ORGAN_DONATION_FIND_OUT_MORE_URL,
    };
  },
};
</script>

<style module lang="scss">
</style>
