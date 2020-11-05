<template>
  <div v-if="showTemplate">
    <proof-level-uplift-banner description="silverIntegration"/>
  </div>
</template>

<script>
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  layout: 'nhsuk-layout',
  components: {
    ProofLevelUpliftBanner,
  },
  mounted() {
    const feature = this.$router.currentRoute.query.featureName;
    EventBus.$emit(UPDATE_HEADER, feature, true, true);
    EventBus.$emit(UPDATE_TITLE, feature, true);
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
