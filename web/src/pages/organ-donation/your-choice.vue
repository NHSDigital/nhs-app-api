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
    <form id="back-form" :action="organDonationPath" method="get">
      <generic-button id="back-to-organdonation"
                      :class="[$style.button, $style.grey]"
                      @click.stop.prevent="goBack">
        {{ $t('organDonation.yourChoice.backButtonText') }}
      </generic-button>
    </form>
  </div>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import RadioButton from '@/components/widgets/RadioButton';
import { ORGAN_DONATION } from '@/lib/routes';
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
    organDonationPath() {
      return ORGAN_DONATION.path;
    },
  },
  created() {
    if (isNil(this.$store.state.organDonation.registration.decisionDetails)) {
      this.$store.dispatch(this.setAllOrgansAction, true);
    }
  },
  methods: {
    goBack() {
      this.$router.push(this.organDonationPath);
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
