<template>
  <div v-if="showTemplate">
    <proof-level-uplift-banner description="appointments"/>
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
