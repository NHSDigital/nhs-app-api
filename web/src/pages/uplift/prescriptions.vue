<template>
  <div v-if="showTemplate">
    <proof-level-uplift-banner description="prescriptions"/>
  </div>
</template>

<script>
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';

export default {
  layout: 'nhsuk-layout',
  components: {
    ProofLevelUpliftBanner,
  },
};
</script>
