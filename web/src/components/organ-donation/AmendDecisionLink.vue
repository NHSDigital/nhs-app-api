<template>
  <a id="amendDecisionLink"
     :class="$style.amendDecision"
     href="#"
     @click.stop.prevent="amendDecision">
    {{ $t('organDonation.links.amendDecisionText') }}
  </a>
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

<style module lang="scss" scoped>

  .amendDecision {
    padding: 0.5em;
    margin-left: -0.5em;
  }
</style>
