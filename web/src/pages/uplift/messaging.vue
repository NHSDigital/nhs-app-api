<template>
  <div v-if="showTemplate">
    <proof-level-uplift-banner/>
  </div>
</template>

<script>
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';

export default {
  name: 'UpliftMessagingPage',
  components: {
    ProofLevelUpliftBanner,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
