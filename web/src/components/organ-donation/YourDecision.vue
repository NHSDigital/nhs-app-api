<template>
  <div>
    <h2>{{ $t(headerKey) }}</h2>
    <div :class="$style['flex-container']">
      <div>
        <component :is="icon"
                   v-if="icon"
                   :class="$style.icon"
                   title-id="decision-text-id"
                   aria-hidden="true" />
      </div>
      <p id="decision-text-id" :class="[style, $style.label, 'nhsuk-heading-m']">
        {{ $t(decisionTextKey) }}
      </p>
    </div>
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
      default: 'organDonation.yourDecision.yourDecision',
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
      decisionTextKey: `organDonation.yourDecision.decisionText.${key}${organsKey}`,
      icon,
      style: this.$style[`${key}-label`],
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/your-decision";
</style>
