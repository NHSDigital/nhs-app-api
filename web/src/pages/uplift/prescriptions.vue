<template>
  <div v-if="showTemplate">
    <proof-level-uplift-banner description="prescriptions"/>
  </div>
</template>

<script>
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';

export default {
  components: {
    ProofLevelUpliftBanner,
  },
  layout: 'nhsuk-layout',
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
