<template>
  <a href="#"
     @click.stop.prevent="amendDecision">{{ $t('organDonation.amendDecision.linkText') }}</a>
</template>

<script>
import { ORGAN_DONATION_AMEND } from '@/lib/routes';

export default {
  name: 'AmendDecisionLink',
  methods: {
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      this.$router.push(ORGAN_DONATION_AMEND.path);
    },
  },
};
</script>

<style scoped>
</style>
