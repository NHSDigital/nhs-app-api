<template>
  <div :class="$style.info">
    <h2>{{ $t(headerKey) }}</h2>
    <p :class="$style['flex-container']">
      <component :is="icon"
                 v-if="icon"
                 :class="$style.icon"
                 title-id="decision-text-id"
                 aria-hidden="true" />
      <span id="decision-text-id" :class="[style, $style.label]">{{ $t(decisionTextKey) }}</span>
    </p>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AppointedRepIcon from '@/components/icons/organ-donation/AppointedRepIcon';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import { DECISION_APPOINTED_REP, DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'YourDecision',
  components: {
    AppointedRepIcon,
    NoIcon,
    YesIcon,
  },
  props: {
    decision: {
      type: String,
      required: true,
    },
    decisionDetails: {
      type: Object,
      default: () => {},
    },
    headerKey: {
      type: String,
      default: 'organDonation.reviewYourDecision.yourDecision.subheader',
    },
    isWithdrawing: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    let icon;
    let key;
    let organsKey = '';

    if (this.isWithdrawing) {
      key = 'withdraw';
    } else {
      const isOptOut = this.decision === DECISION_OPT_OUT;
      const isAppointedRep = this.decision === DECISION_APPOINTED_REP;

      if (isOptOut) {
        key = 'optout';
        icon = NoIcon;
      } else if (isAppointedRep) {
        key = 'appointedrep';
        icon = AppointedRepIcon;
      } else {
        key = 'optin';
        icon = YesIcon;
      }

      if (!isOptOut && !isAppointedRep && !get('all')(this.decisionDetails)) {
        organsKey = 'Some';
      }
    }

    return {
      decisionTextKey: `organDonation.reviewYourDecision.yourDecision.${key}${organsKey}DecisionText`,
      icon,
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
    margin-right: $two;
  }
  .label {
    flex: 1 1 auto;
    font-weight: bold;
    padding-top: $one;
  }
}

.optout-label {
  color: $red;
}
.optin-label {
  color: $light_green;
}
.appointedrep-label {
  color: $nhs_blue;
}
.withdraw-label {
  color: $purple;
}
</style>
