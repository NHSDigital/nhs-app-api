<template>
  <div :class="$style.reaffirmDecision">
    <h3>{{ $t('organDonation.reaffirmDecision.subheader') }}</h3>
    <p>{{ $t('organDonation.reaffirmDecision.text') }}</p>
    <a id="reaffirmDecisionLink" :class="$style['mt-3']" href="#"
       @click.stop.prevent="setReaffirmDecision">
      {{ $t('organDonation.reaffirmDecision.linkText') }}
    </a>
  </div>
</template>

<script>
import {
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_YOUR_CHOICE,
} from '@/lib/routes';

export default {
  name: 'ReaffirmDecision',
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
@import "../../style/spacings";
</style>
