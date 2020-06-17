<template>
  <proof-level-uplift-banner v-if="showTemplate" description="appointments"/>
</template>

<script>
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';

export default {
  name: 'UpliftAppointmentsPage',
  components: {
    ProofLevelUpliftBanner,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>
