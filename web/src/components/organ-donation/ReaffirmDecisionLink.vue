<template>
  <li>
    <a id="reaffirmDecisionLink" href="#"
       @click.stop.prevent="setReaffirmDecision">
      {{ $t('organDonation.links.reaffirmDecisionText') }}
    </a>
  </li>
</template>

<script>
import {
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  ORGAN_DONATION_YOUR_CHOICE_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'ReaffirmDecisionLink',
  props: {
    isSomeOrgans: {
      type: Boolean,
      required: true,
    },
  },
  methods: {
    setReaffirmDecision() {
      this.$store.dispatch('organDonation/reaffirmStart');

      redirectTo(this, this.isSomeOrgans
        ? ORGAN_DONATION_YOUR_CHOICE_PATH
        : ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
    },
  },
};
</script>
