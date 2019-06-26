<template>
  <a id="reaffirmDecisionLink" :class="$style.reaffirmDecision" href="#"
     @click.stop.prevent="setReaffirmDecision">
    {{ $t('organDonation.links.reaffirmDecisionText') }}
  </a>
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

<style module lang="scss" scoped>

.reaffirmDecision {
  padding: 0.5em;
  margin-left: -0.5em;
}
</style>
