<template>
  <analytics-tracked-tag :href="alreadyRegisteredUrl"
                         :text="$t('organDonation.links.alreadyRegisteredText')"
                         tag="a" target="_blank">
    {{ $t('organDonation.links.alreadyRegisteredText') }}
  </analytics-tracked-tag>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'AlreadyRegisteredLink',
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      alreadyRegisteredUrl: this.$store.app.$env.ORGAN_DONATION_ALREADY_REGISTERED_URL,
    };
  },
};
</script>

<style module lang="scss">
</style>
