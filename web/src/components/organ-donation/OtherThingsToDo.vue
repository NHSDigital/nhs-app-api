<template>
  <div>
    <h2>{{ $t('organDonation.otherThings.otherThingsYouCanDo') }}</h2>
    <menu-item-list>
      <menu-item v-if="canWithdraw"
                 id="btn_withdraw"
                 header-tag="h3"
                 href="#"
                 :description="$t('organDonation.otherThings.youCanWithdrawAtAnyTime')"
                 :text="$t('organDonation.otherThings.withdrawYourDecision')"
                 :click-func="withdrawDecision"
                 :aria-label="`${$t('organDonation.otherThings.withdrawYourDecision')}.
                               ${$t('organDonation.otherThings.youCanWithdrawAtAnyTime')}`"/>
      <menu-item id="btn_blood"
                 header-tag="h3"
                 :href="bloodDonationUrl"
                 target="_blank"
                 :description="$t('organDonation.otherThings.youCouldSaveLivesByGivingBlood')"
                 :text="$t('organDonation.otherThings.registerToBeABloodDonor')"
                 :aria-label="`${$t('organDonation.otherThings.registerToBeABloodDonor')}.
                               ${$t('organDonation.otherThings.youCouldSaveLivesByGivingBlood')}`"/>
    </menu-item-list>
  </div>
</template>

<script>
import { ORGAN_DONATION_WITHDRAW_REASON_PATH } from '@/router/paths';
import MenuItemList from '@/components/MenuItemList';
import MenuItem from '@/components/MenuItem';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'OtherThingsToDo',
  components: {
    MenuItem,
    MenuItemList,
  },
  props: {
    canWithdraw: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      bloodDonationUrl: this.$store.$env.BLOOD_DONATION_URL,
    };
  },
  methods: {
    withdrawDecision() {
      this.$store.dispatch('organDonation/withdrawStart');
      redirectTo(this, ORGAN_DONATION_WITHDRAW_REASON_PATH);
    },
  },
};
</script>
