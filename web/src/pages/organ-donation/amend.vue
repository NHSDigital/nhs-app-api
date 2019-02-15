<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <make-decision/>
    <generic-button id="back-button"
                    :class="[$style.button, $style.grey]"
                    @click="goBack" >
      {{ $t('generic.backButton.text') }}
    </generic-button>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import { ORGAN_DONATION } from '@/lib/routes';

export default {
  components: {
    GenericButton,
    MakeDecision,
  },
  fetch({ redirect, store }) {
    if (!store.state.organDonation.isAmending) {
      redirect(ORGAN_DONATION.path);
    }
  },
  methods: {
    goBack() {
      this.$store.dispatch('organDonation/amendCancel');
      this.$router.push(ORGAN_DONATION.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
</style>
