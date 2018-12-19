<template>
  <no-js-form :action="formAction" :value="noJsValue">
    <button :class="$style['no-button']" @click.prevent="chooseDecision()">
      <no-icon/>
      <h2>{{ $t('organDonation.register.noButton.header') }}</h2>
      <p>{{ $t('organDonation.register.noButton.subheader') }}</p>
    </button>
  </no-js-form>
</template>

<script>
import { ORGAN_DONATION } from '@/lib/routes';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import NoJsForm from '@/components/no-js/NoJsForm';

export default {
  name: 'OrganDonationButton',
  components: {
    NoIcon,
    NoJsForm,
  },
  props: {
    decision: {
      type: String,
      required: true,
    },
  },
  computed: {
    formAction() {
      return ORGAN_DONATION.path;
    },
    noJsValue() {
      return { organDonation: { registration: { decision: this.decision } } };
    },
  },
  methods: {
    chooseDecision() {
      this.$store.dispatch('organDonation/makeDecision', this.decision);
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/colours";
  @import "../../style/spacings";

  .no-button {
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

    h2 {
      color: $red;
      font-size: 2em;
      margin: 0;
      padding: 0;
    }

    p {
      color: $red;
      font-size: 1.5em;
    }
  }
</style>
