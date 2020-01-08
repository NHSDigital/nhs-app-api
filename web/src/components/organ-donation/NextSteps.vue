<template>
  <div>
    <h2>{{ $t('organDonation.viewDecision.nextSteps.subheader') }}</h2>
    <menu-item-list>
      <menu-item id="btn_tellFamily"
                 header-tag="h3"
                 role="link"
                 target="_blank"
                 :href="tellFamilyUrl"
                 :description="$t('organDonation.viewDecision.nextSteps.tellFamily.body')"
                 :text="$t('organDonation.viewDecision.nextSteps.tellFamily.subheader')"
                 :aria-label="`${$t('organDonation.viewDecision.nextSteps.tellFamily.subheader')}.
                                 ${$t('organDonation.viewDecision.nextSteps.tellFamily.body')}`"/>

      <menu-item v-if="isOptInDecision"
                 id="btn_shareDecision"
                 header-tag="h3"
                 role="link"
                 target="_blank"
                 :href="shareDecisionUrl"
                 :description="$t('organDonation.viewDecision.nextSteps.shareDecision.body')"
                 :text="$t('organDonation.viewDecision.nextSteps.shareDecision.subheader')"
                 :aria-label="`${$t(
                   'organDonation.viewDecision.nextSteps.shareDecision.subheader')}.
                   ${$t('organDonation.viewDecision.nextSteps.shareDecision.body')}`"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';

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
      shareDecisionUrl: this.$store.app.$env.ORGAN_DONATION_SHARE_DECISION_URL,
      tellFamilyUrl: this.$store.app.$env.ORGAN_DONATION_TELL_FAMILY_URL,
    };
  },
};
</script>
