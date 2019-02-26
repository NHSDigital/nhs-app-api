<template>
  <a href="#"
     @click.stop.prevent="amendDecision">{{ $t('organDonation.links.amendText') }}</a>
</template>

<script>
import { ORGAN_DONATION_AMEND } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'AmendDecisionLink',
  methods: {
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      redirectTo(this, ORGAN_DONATION_AMEND.path, null);
    },
  },
};
</script>

<style scoped>
</style>
