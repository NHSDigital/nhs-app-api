<template>
  <div>
    <h3>{{ $t('organDonation.viewDecision.otherThings.subheader') }}</h3>
    <ul :class="$style['list-menu']">
      <li role="link">
        <analytics-tracked-tag id="btn_blood" :href="bloodDonationUrl"
                               :class="$style['no-decoration']"
                               :text="$t(
                               'organDonation.viewDecision.otherThings.bloodDonation.subheader')"
                               :aria-label="`${$t(
                               'organDonation.viewDecision.otherThings.bloodDonation.subheader')}.
                               ${$t('organDonation.viewDecision.otherThings.bloodDonation.body')}`"
                               tag="a" target="_blank">
          <h2>{{ $t('organDonation.viewDecision.otherThings.bloodDonation.subheader') }}</h2>
          <p>{{ $t('organDonation.viewDecision.otherThings.bloodDonation.body') }}</p>
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>


<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      bloodDonationUrl: this.$store.app.$env.BLOOD_DONATION_URL,
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
