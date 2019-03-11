<template>
  <div>
    <h2>{{ $t('organDonation.otherThings.subheader') }}</h2>
    <ul :class="$style['list-menu']">
      <li v-if="canWithdraw">
        <a id="btn_withdraw" :class="[$style.noDecoration, $style.focusBorder]" href="#"
           @click.stop.prevent="withdrawDecision">
          <h3>{{ $t('organDonation.otherThings.withdraw.subheader') }}</h3>
          <p>{{ $t('organDonation.otherThings.withdraw.body') }}</p>
        </a>
      </li>
      <li>
        <analytics-tracked-tag id="btn_blood" :href="bloodDonationUrl"
                               :class="[$style.noDecoration, $style.focusBorder]"
                               :text="$t('organDonation.otherThings.bloodDonation.subheader')"
                               :aria-label="`${$t(
                                 'organDonation.otherThings.bloodDonation.subheader')}.
                               ${$t('organDonation.otherThings.bloodDonation.body')}`"
                               tag="a" target="_blank">
          <h3>{{ $t('organDonation.otherThings.bloodDonation.subheader') }}</h3>
          <p>{{ $t('organDonation.otherThings.bloodDonation.body') }}</p>
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { ORGAN_DONATION_WITHDRAW_REASON } from '@/lib/routes';

export default {
  name: 'OtherThingsToDo',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    canWithdraw: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      bloodDonationUrl: this.$store.app.$env.BLOOD_DONATION_URL,
    };
  },
  methods: {
    withdrawDecision() {
      this.$store.dispatch('organDonation/withdrawStart');
      this.$router.push(ORGAN_DONATION_WITHDRAW_REASON.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/accessibility";
  @import "../../style/listmenu";

  .noDecoration {
    text-decoration: none;
  }

  .list-menu {
    li {
      a {
        h3 {
          @include h2;
          color: $nhs_blue;
        }
      }
    }
  }

</style>
