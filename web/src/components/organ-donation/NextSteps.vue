<template>
  <div>
    <h2>{{ $t('organDonation.nextSteps.nextSteps') }}</h2>
    <menu-item-list>
      <menu-item id="btn_tellFamily"
                 header-tag="h3"
                 target="_blank"
                 :href="tellFamilyUrl"
                 :description="$t('organDonation.nextSteps.doYourFamiltyAndFriendsKnow')"
                 :text="$t('organDonation.nextSteps.tellYourFamilyAndFriends')"
                 :aria-label="`${$t('organDonation.nextSteps.tellYourFamilyAndFriends')}.
                                 ${$t('organDonation.nextSteps.doYourFamiltyAndFriendsKnow')}`"/>

      <menu-item v-if="isOptInDecision"
                 id="btn_shareDecision"
                 header-tag="h3"
                 target="_blank"
                 :href="shareDecisionUrl"
                 :description="$t('organDonation.nextSteps.helpPromoteOnSocialMedia')"
                 :text="$t('organDonation.nextSteps.shareThatYouAreADonor')"
                 :aria-label="`${$t(
                   'organDonation.nextSteps.shareThatYouAreADonor')}.
                   ${$t('organDonation.nextSteps.helpPromoteOnSocialMedia')}`"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import {
  ORGAN_DONATION_SHARE_DECISION_URL,
  ORGAN_DONATION_TELL_FAMILY_URL,
} from '@/router/externalLinks';

export default {
  name: 'NextSteps',
  components: {
    MenuItem,
    MenuItemList,
  },
  props: {
    isOptInDecision: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      shareDecisionUrl: ORGAN_DONATION_SHARE_DECISION_URL,
      tellFamilyUrl: ORGAN_DONATION_TELL_FAMILY_URL,
    };
  },
};
</script>
