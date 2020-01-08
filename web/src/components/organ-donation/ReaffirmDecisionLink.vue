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
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_YOUR_CHOICE,
} from '@/lib/routes';

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
      this.$router.push(this.isSomeOrgans
        ? ORGAN_DONATION_YOUR_CHOICE.path
        : ORGAN_DONATION_REVIEW_YOUR_DECISION.path);
    },
  },
};
</script>
