<template>
  <proof-level-uplift-banner v-if="showTemplate" description="more"/>
</template>

<script>
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';

export default {
  name: 'UpliftMorePage',
  components: {
    ProofLevelUpliftBanner,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
