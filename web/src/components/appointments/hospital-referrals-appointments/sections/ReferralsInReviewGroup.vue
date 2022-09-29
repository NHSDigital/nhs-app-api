<template>
  <div>
    <h2 id="referrals-in-review-title"
        class="nhsuk-u-padding-bottom-5">
      {{ groupTitleWithCounter(totalReferralsInReviewNotOverdue) }}
    </h2>
    <help-link
      id="wayfinder-help-jump-off-link-referrals"
      :href="wayfinderHelpPath"
      :click-func="redirectToWayfinderHelp"
      :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.referrals')"/>
    <card-group v-if="hasAny"
                class="nhsuk-grid-row">
      <card-group-item v-for="(referral, index) in referralsInReviewNotOverdue"
                       :key="`referral-in-review-not-overdue-${index}`"
                       class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
        <referral-in-review-card :item="referral"/>
      </card-group-item>
    </card-group>
    <!--Card group is currently the only way to get this to act as the other groups style wise !-->
    <card-group v-else class="nhsuk-grid-row">
      <card-group-item class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
        <p
          id="no-referrals-in-review-text">
          {{ $t('wayfinder.noReferralsInReview') }}
        </p>
      </card-group-item>
    </card-group>
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
