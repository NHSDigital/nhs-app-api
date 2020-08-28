<template>
  <li>
    <a id="amendDecisionLink"
       href="#"
       @click.stop.prevent="amendDecision">
      {{ $t('organDonation.iWantToChangeMyDecision') }}
    </a>
  </li>
</template>

<script>
import { ORGAN_DONATION_AMEND_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'AmendDecisionLink',
  methods: {
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      redirectTo(this, ORGAN_DONATION_AMEND_PATH);
    },
  },
};
</script>
