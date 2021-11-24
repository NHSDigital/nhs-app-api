<template>
  <div>
    <h3>{{ $t('organDonation.stillYourDecision.isThisStillYourDecision') }}</h3>
    <p>{{ $t('organDonation.stillYourDecision.keepingUpToDateWillHelp') }}</p>
    <reaffirm-decision-link v-if="showReaffirm"
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
