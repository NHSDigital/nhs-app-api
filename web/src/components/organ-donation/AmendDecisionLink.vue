<template>
  <li>
    <a id="amendDecisionLink"
       href="#"
       @click.stop.prevent="amendDecision">
      {{ $t('organDonation.links.amendDecisionText') }}
    </a>
  </li>
</template>

<script>
import { ORGAN_DONATION_AMEND } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'AmendDecisionLink',
  methods: {
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      redirectTo(this, ORGAN_DONATION_AMEND.path);
    },
  },
};
</script>
