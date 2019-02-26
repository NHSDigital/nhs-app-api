<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <make-decision/>
    <ul :class="$style['list-menu']">
      <li>
        <find-out-more-link/>
      </li>
    </ul>
    <generic-button id="back-button"
                    :class="[$style.button, $style.grey]"
                    @click="goBack" >
      {{ $t('generic.backButton.text') }}
    </generic-button>
  </div>
</template>

<script>
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import GenericButton from '@/components/widgets/GenericButton';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import { ORGAN_DONATION } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    FindOutMoreLink,
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
      redirectTo(this, ORGAN_DONATION.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/listmenu";
@import "../../style/spacings";

.list-menu {
  margin-bottom: $three;
}
</style>
