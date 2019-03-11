<template>
  <div :class="$style['mb-3']">
    <h3>{{ $t('organDonation.stillYourDecision.subheader') }}</h3>
    <p :class="$style['mb-1']">{{ $t('organDonation.stillYourDecision.text') }}</p>
    <reaffirm-decision-link :class="$style['mb-1']"
                            v-if="showReaffirm"
                            :is-some-organs="isSomeOrgans"/>
    <amend-decision-link v-if="showAmend"/>
  </div>
</template>

<script>
import AmendDecisionLink from '@/components/organ-donation/AmendDecisionLink';
import ReaffirmDecisionLink from '@/components/organ-donation/ReaffirmDecisionLink';

export default {
  name: 'StillYourDecision',
  components: {
    AmendDecisionLink,
    ReaffirmDecisionLink,
  },
  props: {
    isSomeOrgans: {
      type: Boolean,
      default: false,
    },
    showAmend: {
      type: Boolean,
      required: true,
    },
    showReaffirm: {
      type: Boolean,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
</style>
