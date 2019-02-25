<template>
  <div>
    <h3>{{ $t('organDonation.viewDecision.nextSteps.subheader') }}</h3>
    <div v-if="isOptInDecision">
      <ul :class="$style['list-menu']">
        <li role="link">
          <analytics-tracked-tag id="btn_shareDecision" :href="shareDecisionUrl"
                                 :class="$style['no-decoration']"
                                 :text="$t(
                                 'organDonation.viewDecision.nextSteps.shareDecision.subheader')"
                                 :aria-label="`${$t(
                                 'organDonation.viewDecision.nextSteps.shareDecision.subheader')}.
                                 ${$t('organDonation.viewDecision.nextSteps.shareDecision.body')}`"
                                 tag="a" target="_blank">
            <h2>{{ $t('organDonation.viewDecision.nextSteps.shareDecision.subheader') }}</h2>
            <p>{{ $t('organDonation.viewDecision.nextSteps.shareDecision.body') }}</p>
          </analytics-tracked-tag>
        </li>
        <li role="link">
          <analytics-tracked-tag id="btn_tellFamily" :href="tellFamilyUrl"
                                 :class="$style['no-decoration']"
                                 :text="$t(
                                 'organDonation.viewDecision.nextSteps.tellFamily.subheader')"
                                 :aria-label="`${$t(
                                 'organDonation.viewDecision.nextSteps.tellFamily.subheader')}.
                                 ${$t('organDonation.viewDecision.nextSteps.tellFamily.body')}`"
                                 tag="a" target="_blank">
            <h2>{{ $t('organDonation.viewDecision.nextSteps.tellFamily.subheader') }}</h2>
            <p>{{ $t('organDonation.viewDecision.nextSteps.tellFamily.body') }}</p>
          </analytics-tracked-tag>
        </li>
      </ul>
    </div>
    <div v-else>
      <p>{{ $t('organDonation.viewDecision.nextSteps.optOutText') }}</p>
    </div>
  </div>
</template>


<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'NextSteps',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    isOptInDecision: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      shareDecisionUrl: this.$store.app.$env.ORGAN_DONATION_SHARE_DECISION_URL,
      tellFamilyUrl: this.$store.app.$env.ORGAN_DONATION_TELL_FAMILY_URL,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/listmenu";
  @import '../../style/buttons';

  .no-decoration {
    text-decoration: none;
  }

</style>
