<template>
  <div :class="$style['flex-container']">
    <button :class="[style, $style['decision-button']]" @click.prevent="chooseDecision()">
      <div>
        <component :is="icon" :class="$style['button-content']" :title-id="headerId" />
        <h2 :id="headerId" aria-hidden="true">{{ $t(headerKey) }}</h2>
        <p>{{ $t(subHeaderKey) }}</p>
      </div>
    </button>
  </div>
</template>

<script>
import { ORGAN_DONATION_ADDITIONAL_DETAILS, ORGAN_DONATION_YOUR_CHOICE } from '@/lib/routes';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import { DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';
import { redirectTo } from '@/lib/utils';

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
        ? ORGAN_DONATION_ADDITIONAL_DETAILS.path
        : ORGAN_DONATION_YOUR_CHOICE.path,
      style: isOptOut ? this.$style['no-button'] : this.$style['yes-button'],
      headerKey: isOptOut ? 'organDonation.register.noButton.header' : 'organDonation.register.yesButton.header',
      subHeaderKey: isOptOut ? 'organDonation.register.noButton.subheader' : 'organDonation.register.yesButton.subheader',
      icon: isOptOut ? NoIcon : YesIcon,
      headerId: `${isOptOut ? 'no' : 'yes'}-header-id`,
    };
  },
  methods: {
    chooseDecision() {
      this.$store.dispatch('organDonation/makeDecision', this.decision);
      redirectTo(this, this.nextRoute, null);
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/colours";
  @import "../../style/spacings";

  .decision-button {
    @include space(padding, top, $three);
    @include space(padding, left, $four);
    @include space(padding, right, $four);
    @include space(padding, bottom, $three);
    background: $white;
    border: none;
    cursor: pointer;
    outline: none;
    width: 100%;
    height: 100%;

    div {
      height: 100%;
    }

    h2 {
      font-size: 2em;
      margin: 0;
      padding: 0;
    }

    p {
      font-size: 1.5em;
    }
  }

  .no-button {

    h2 {
      color: $red;
    }

    p {
      color: $red;
    }
  }

  .yes-button {

    h2 {
      color: $light_green;
    }

    p {
      color: $light_green;
    }
  }

  .button-content {
    vertical-align: top;
  }

  .flex-container {
    flex: 0 50%;
  }
</style>
