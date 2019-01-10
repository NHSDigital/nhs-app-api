<template>
  <div :class="$style.info">
    <h2>{{ $t('organDonation.reviewYourDecision.yourDecision.subheader') }}</h2>
    <p :class="$style['flex-container']">
      <component :class="$style.icon" :is="icon"/>
      <span :class="[style, $style.label]">{{ $t(decisionTextKey) }}</span>
    </p>
  </div>
</template>

<script>
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import { DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'YourDecision',
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
    const key = isOptOut ? 'optout' : 'optin';
    return {
      decisionTextKey: `organDonation.reviewYourDecision.yourDecision.${key}DecisionText`,
      icon: isOptOut ? NoIcon : YesIcon,
      style: this.$style[`${key}-label`],
    };
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/colours";
@import "../../style/spacings";

.flex-container {
  display: flex;
  align-content: flex-start;

  .icon {
    flex: 0 0 2.5em;
  }
  .label {
    flex: 1 1 auto;
    font-weight: bold;
    padding-left: $two;
    padding-top: $one;
  }
}

.optout-label {
    color: $red;
}
.optin-label {
    color: $light_green;
}
</style>
