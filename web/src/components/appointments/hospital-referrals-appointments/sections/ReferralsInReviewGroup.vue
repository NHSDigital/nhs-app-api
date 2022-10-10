<template>
  <div>
    <h2 id="referrals-in-review-title"
        class="nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
      {{ groupTitleWithCounter(totalReferralsInReviewNotOverdue) }}
    </h2>
    <div class="nhsuk-u-padding-bottom-2">
      <help-link
        id="wayfinder-help-jump-off-link-referrals"
        :path="wayfinderHelpPath"
        :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.referrals')"/>
    </div>
    <card-group v-if="hasAny"
                class="nhsuk-grid-row nhsuk-u-padding-bottom-3 nhsuk-u-margin-bottom-0"
                :class="!hasAny ? 'nhsuk-u-padding-bottom-0' : ''">
      <card-group-item v-for="(referral, index) in referralsInReviewNotOverdue"
                       :key="`referral-in-review-not-overdue-${index}`"
                       class="nhsuk-grid-column-full nhsuk-u-padding-bottom-4">
        <referral-in-review-card :item="referral"/>
      </card-group-item>
    </card-group>
    <div v-else class="nhsuk-u-padding-bottom-4"/>
  </div>
</template>

<script>
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import ReferralInReviewCard from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralInReviewCard';
import { isNonEmptyArray, redirectTo } from '@/lib/utils';
import { WAYFINDER_HELP_PATH } from '@/router/paths';

export default {
  name: 'ReferralsInReviewGroup',
  components: {
    ReferralInReviewCard,
    CardGroup,
    CardGroupItem,
    HelpLink,
  },
  data() {
    return {
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
    };
  },
  computed: {
    hasAny() {
      return isNonEmptyArray(this.referralsInReviewNotOverdue);
    },
    referralsInReviewNotOverdue() {
      return this.$store.state.wayfinder.summary.referralsInReviewNotOverdue;
    },
    totalReferralsInReviewNotOverdue() {
      return this.$store.state.wayfinder.summary.referralsInReviewNotOverdue.length;
    },
  },
  methods: {
    redirectToWayfinderHelp() {
      redirectTo(this, this.wayfinderHelpPath);
    },
    groupTitleWithCounter(count) {
      if (count === 0 || count > 1) {
        return this.$t('wayfinder.inReviewTitle')
          .replace('{count}', count)
          .replace('{plural}', 's');
      }

      return this.$t('wayfinder.inReviewTitle')
        .replace('{count}', count)
        .replace('{plural}', '');
    },
  },
};
</script>
