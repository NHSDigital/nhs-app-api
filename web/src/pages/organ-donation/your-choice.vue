<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="showErrors" message-type="error">
      <message-text data-purpose="error-heading">
        {{ $t('organDonation.yourChoice.errorMessageHeader') }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li>{{ $t('organDonation.yourChoice.errorMessageText') }}</li>
      </message-list>
    </message-dialog>
    <div :class="$style.info">
      <h2>{{ $t('organDonation.yourChoice.subheader') }}</h2>
      <p>{{ $t('organDonation.yourChoice.description') }}</p>
    </div>
    <radio-group v-model="selectedValue"
                 :class="$style.radioGroup"
                 :current-value="currentChoice"
                 :radios="radioButtons"
                 :show-error="showErrors"
                 :error-message="$t('organDonation.yourChoice.errorMessageText')"
                 @select="selected"/>
    <generic-button id="continue-button"
                    :class="['nhsuk-button']"
                    @click.prevent="continueClicked">
      {{ $t('organDonation.yourChoice.continueButtonText') }}
    </generic-button>
    <back-button v-if="!$store.state.device.isNativeApp" :before="beforeBack" />
  </div>
</template>
<script>
import get from 'lodash/fp/get';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import isNil from 'lodash/fp/isNil';
import BackButton from '@/components/BackButton';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RadioGroup from '@/components/RadioGroup';
import { EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import {
  ORGAN_DONATION_FAITH,
  ORGAN_DONATION_SOME_ORGANS,
} from '@/lib/routes';

export default {
  components: {
    BackButton,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    RadioGroup,
  },
  mixins: [EnsureOptInDecision],
  data() {
    return {
      hasTriedToContinue: false,
      radioButtons: [
        {
          hint: this.$t('organDonation.yourChoice.choices.all.description'),
          label: this.$t('organDonation.yourChoice.choices.all.title'),
          value: true,
        },
        {
          hint: this.$t('organDonation.yourChoice.choices.some.description'),
          label: this.$t('organDonation.yourChoice.choices.some.title'),
          value: false,
        },
      ],
      setAllOrgansAction: 'organDonation/setAllOrgans',
      selectedValue: undefined,
    };
  },
  computed: {
    currentChoice() {
      return get('$store.state.organDonation.registration.decisionDetails.all')(this);
    },
    hasMadeDecision() {
      return !(this.currentChoice === '' || this.currentChoice === undefined);
    },
    showErrors() {
      return this.hasTriedToContinue && !this.hasMadeDecision;
    },
  },
  asyncData({ store }) {
    if (
      store.state.organDonation.isAmending &&
      isDefault({
        path: 'registration.decisionDetails.all',
        state: store.state.organDonation,
      })
    ) {
      store.dispatch('organDonation/cloneFromOriginal', 'decisionDetails.all');
    }
  },
  created() {
    if (isNil(this.$store.state.organDonation.registration.decisionDetails)) {
      this.$store.dispatch(this.setAllOrgansAction, '');
    }
  },
  methods: {
    beforeBack() {
      this.hasTriedToContinue = false;
    },
    continueClicked() {
      this.hasTriedToContinue = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }

      this.$router.push(this.currentChoice ?
        ORGAN_DONATION_FAITH.path :
        ORGAN_DONATION_SOME_ORGANS.path);
    },
    selected(value) {
      this.$store.dispatch(this.setAllOrgansAction, value);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";
@import "../../style/spacings";
</style>
