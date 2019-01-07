<template>
  <no-js-form :action="formAction" :value="noJsValue">
    <button :class="[style, $style['decision-button']]" @click.prevent="chooseDecision()">
      <component :is="icon"/>
      <h2>{{ $t(headerKey) }}</h2>
      <p>{{ $t(subHeaderKey) }}</p>
    </button>
  </no-js-form>
</template>

<script>
import { ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_YOUR_CHOICE } from '@/lib/routes';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import NoJsForm from '@/components/no-js/NoJsForm';
import { DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';

export default {
  name: 'OrganDonationButton',
  components: {
    NoIcon,
    YesIcon,
    NoJsForm,
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
      formAction: isOptOut ? ORGAN_DONATION_ADDITIONAL_DETAILS.path
        : ORGAN_DONATION_YOUR_CHOICE.path,
      style: isOptOut ? this.$style['no-button'] : this.$style['yes-button'],
      headerKey: isOptOut ? 'organDonation.register.noButton.header' : 'organDonation.register.yesButton.header',
      subHeaderKey: isOptOut ? 'organDonation.register.noButton.subheader' : 'organDonation.register.yesButton.subheader',
      noJsValue: { organDonation: { registration: { decision: this.decision } } },
      icon: isOptOut ? NoIcon : YesIcon,
    };
  },
  methods: {
    chooseDecision() {
      this.$store.dispatch('organDonation/makeDecision', this.decision);
      this.$router.push(this.formAction);
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
    width: 50%;
    max-width: 175px;
    float: left;

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
    margin-right: $one;

    h2 {
      color: $red;
    }

    p {
      color: $red;
    }
  }

  .yes-button {
    margin-left: $one;

    h2 {
      color: $light_green;
    }

    p {
      color: $light_green;
    }
  }
</style>
