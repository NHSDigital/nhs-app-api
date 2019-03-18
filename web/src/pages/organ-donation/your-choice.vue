<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="hasTriedToContinue && !hasMadeDecision" message-type="error">
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
    <generic-radio-button
      :class="$style['radio-button']"
      :name="allOrgansName"
      :value="true"
      :model="currentChoice"
      @select="isSelected">
      <b>{{ $t('organDonation.yourChoice.choices.all.title') }}</b>
      <p>{{ $t('organDonation.yourChoice.choices.all.description') }}</p>
    </generic-radio-button>
    <generic-radio-button
      :class="$style['radio-button']"
      :name="allOrgansName"
      :value="false"
      :model="currentChoice"
      @select="isSelected">
      <b>{{ $t('organDonation.yourChoice.choices.some.title') }}</b>
      <p>{{ $t('organDonation.yourChoice.choices.some.description') }}</p>
    </generic-radio-button>
    <generic-button id="continue-button"
                    :class="[$style.button, $style.green]"
                    @click.prevent="continueClicked">
      {{ $t('organDonation.yourChoice.continueButtonText') }}
    </generic-button>
    <back-button />
  </div>
</template>
<script>
import get from 'lodash/fp/get';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import isNil from 'lodash/fp/isNil';
import BackButton from '@/components/BackButton';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import { EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import {
  ORGAN_DONATION_FAITH,
  ORGAN_DONATION_SOME_ORGANS,
} from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    BackButton,
    ErrorMessage,
    GenericButton,
    GenericRadioButton,
    MessageDialog,
    MessageList,
    MessageText,
  },
  mixins: [EnsureOptInDecision],
  data() {
    return {
      setAllOrgansAction: 'organDonation/setAllOrgans',
      hasTriedToContinue: false,
      allOrgansName: 'AllOrgans',
    };
  },
  computed: {
    currentChoice() {
      return get('$store.state.organDonation.registration.decisionDetails.all')(this);
    },
    hasMadeDecision() {
      return !(this.currentChoice === '' || this.currentChoice === undefined);
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
    isSelected(value) {
      this.$store.dispatch(this.setAllOrgansAction, value);
    },
    continueClicked() {
      this.hasTriedToContinue = true;
      if (this.hasMadeDecision && this.currentChoice === false) {
        redirectTo(this, ORGAN_DONATION_SOME_ORGANS.path, null);
        return;
      }
      if (this.hasMadeDecision && this.currentChoice === true) {
        redirectTo(this, ORGAN_DONATION_FAITH.path, null);
        return;
      }

      window.scrollTo(0, 0);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";
@import "../../style/spacings";

.radio-button {
  margin-bottom: $three;
}
</style>
