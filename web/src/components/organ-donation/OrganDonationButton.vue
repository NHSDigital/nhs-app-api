<template>
  <div>
    <button :class="[style, $style['decision-button']]" @click.prevent="chooseDecision()">
      <div>
        <component :is="icon" :class="$style['button-content', 'icon']" :title-id="headerId" />
        <h2 :id="headerId" class="nhsuk-heading-l" aria-hidden="true">{{ $t(headerKey) }}</h2>
        <p class="nhsuk-heading-m">{{ $t(subHeaderKey) }}</p>
      </div>
    </button>
  </div>
</template>

<script>
import { ORGAN_DONATION_ADDITIONAL_DETAILS_PATH, ORGAN_DONATION_YOUR_CHOICE_PATH } from '@/router/paths';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import { DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';
import { redirectTo } from '@/lib/utils';
import YesIcon from '@/components/icons/organ-donation/YesIcon';

export default {
  name: 'OrganDonationButton',
  components: {
    NoIcon,
    YesIcon,
  },
  props: {
    decision: {
      type: String,
      required: true,
    },
  },
  data() {
    const isOptOut = this.decision === DECISION_OPT_OUT;

    return {
      nextRoute: isOptOut
        ? ORGAN_DONATION_ADDITIONAL_DETAILS_PATH
        : ORGAN_DONATION_YOUR_CHOICE_PATH,
      style: isOptOut ? this.$style['no-button'] : this.$style['yes-button'],
      headerKey: isOptOut ? 'organDonation.button.iDoNotWantToDonate.header' : 'organDonation.button.iDoWantToDonate.header',
      subHeaderKey: isOptOut ? 'organDonation.button.iDoNotWantToDonate.subheader' : 'organDonation.button.iDoWantToDonate.subheader',
      icon: isOptOut ? NoIcon : YesIcon,
      headerId: `${isOptOut ? 'no' : 'yes'}-header-id`,
    };
  },
  methods: {
    chooseDecision() {
      this.$store.dispatch('organDonation/makeDecision', this.decision);
      redirectTo(this, this.nextRoute);
    },
  },
};
</script>

<style module lang="scss">
  @import "@/style/custom/organ-donation-button";
</style>
