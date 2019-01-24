<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div :class="$style.info">
      <h2>{{ $t('organDonation.yourChoice.subheader') }}</h2>
      <p>{{ $t('organDonation.yourChoice.description') }}</p>
    </div>
    <radio-button :action="setAllOrgansAction"
                  :state="'organDonation.registration.decisionDetails.all'"
                  :value="true">
      <b>{{ $t('organDonation.yourChoice.choices.all.title') }}</b>
      <p>{{ $t('organDonation.yourChoice.choices.all.description') }}</p>
    </radio-button>
    <generic-button id="continue-button"
                    :class="[$style.button, $style.green]"
                    @click.stop.prevent="continueClicked">
      {{ $t('organDonation.yourChoice.continueButtonText') }}
    </generic-button>
    <generic-button id="back-to-organdonation"
                    :class="[$style.button, $style.grey]"
                    @click.stop.prevent="goBack">
      {{ $t('organDonation.yourChoice.backButtonText') }}
    </generic-button>
  </div>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import RadioButton from '@/components/widgets/RadioButton';
import { ORGAN_DONATION, ORGAN_DONATION_FAITH } from '@/lib/routes';
import isNil from 'lodash/fp/isNil';
import get from 'lodash/fp/get';

export default {
  components: {
    GenericButton,
    RadioButton,
  },
  data() {
    return {
      setAllOrgansAction: 'organDonation/setAllOrgans',
    };
  },
  computed: {
    currentChoice() {
      return get('all')(this.$store.state.organDonation.registration.decisionDetails);
    },
  },
  created() {
    if (isNil(this.$store.state.organDonation.registration.decisionDetails)) {
      this.$store.dispatch(this.setAllOrgansAction, '');
    }
  },
  methods: {
    goBack() {
      this.$router.push(ORGAN_DONATION.path);
    },
    continueClicked() {
      this.$router.push(ORGAN_DONATION_FAITH.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";

noscript {
  b {
    display: inline;
  }
}
</style>
