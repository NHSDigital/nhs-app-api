<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="showError" message-type="error">
      <message-text data-purpose="error-heading">
        {{ $t('organDonation.faith.errorMsgHeader') }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li>{{ $t('organDonation.faith.errorMsgText') }}</li>
      </message-list>
    </message-dialog>
    <h2>{{ $t('organDonation.faith.subheader') }}</h2>
    <div :class="$style.info">
      <h3>{{ $t('organDonation.faith.why.header') }}</h3>
      <p v-for="(paragraph, index) in $t('organDonation.faith.why.paragraphs')" :key="index">
        {{ paragraph }}
      </p>
    </div>
    <div :class="$style.info">
      <h3>{{ $t('organDonation.faith.help.header') }}</h3>
      <p>{{ $t('organDonation.faith.help.description') }}</p>
    </div>
    <div :class="$style.info">
      <p><b>{{ $t('organDonation.faith.choices.header') }}</b></p>
    </div>
    <generic-radio-button :class="$style.choiceRadioButton"
                          :label="$t('organDonation.faith.choices.yes.title')"
                          :description="$t('organDonation.faith.choices.yes.description')"
                          :model="currentChoice"
                          :value="yesValue"
                          name="choice"
                          @select="radioButtonSelected">
      <p>{{ $t('organDonation.faith.choices.yes.description') }}</p>
    </generic-radio-button>
    <generic-radio-button :class="$style.choiceRadioButton"
                          :label="$t('organDonation.faith.choices.no.title')"
                          :description="$t('organDonation.faith.choices.no.description')"
                          :model="currentChoice"
                          :value="noValue"
                          name="choice"
                          @select="radioButtonSelected">
      <p>{{ $t('organDonation.faith.choices.no.description') }}</p>
    </generic-radio-button>
    <generic-radio-button :class="$style.choiceRadioButton"
                          :label="$t('organDonation.faith.choices.preferNotToSay.title')"
                          :model="currentChoice"
                          :value="preferNotToSayValue"
                          name="choice"
                          @select="radioButtonSelected" />
    <generic-button id="continue-to-additional-details"
                    :class="[$style.button, $style.green]"
                    @click.stop.prevent="continueClicked">
      {{ $t('organDonation.faith.continueButtonText') }}
    </generic-button>

    <back-button />
  </div>
</template>

<script>
import BackButton from '@/components/BackButton';
import GenericButton from '@/components/widgets/GenericButton';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import { YES, NO, NOT_STATED } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_ADDITIONAL_DETAILS } from '@/lib/routes';
import { EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    BackButton,
    GenericButton,
    GenericRadioButton,
    MessageDialog,
    MessageList,
    MessageText,
  },
  mixins: [EnsureOptInDecision],
  data() {
    return {
      yesValue: YES,
      noValue: NO,
      preferNotToSayValue: NOT_STATED,
      hasTriedToContinue: false,
    };
  },
  asyncData({ store }) {
    if (
      store.state.organDonation.isAmending &&
      isDefault({
        path: 'registration.faithDeclaration',
        state: store.state.organDonation,
      })
    ) {
      store.dispatch('organDonation/cloneFromOriginal', 'faithDeclaration');
    }
  },
  computed: {
    currentChoice() {
      return this.$store.state.organDonation.registration.faithDeclaration;
    },
    hasMadeDecision() {
      return !!this.currentChoice;
    },
    showError() {
      return this.hasTriedToContinue && !this.hasMadeDecision;
    },
  },
  methods: {
    radioButtonSelected(value) {
      this.$store.dispatch('organDonation/setFaithDeclaration', value);
    },
    continueClicked() {
      this.hasTriedToContinue = true;
      if (this.showError) {
        window.scrollTo(0, 0);
      } else {
        redirectTo(this, ORGAN_DONATION_ADDITIONAL_DETAILS.path, null);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";

.choiceRadioButton {
  margin-top: 0.5em;
  margin-bottom: 1em;
  &:last-of-type {
    margin-bottom: 1.5em;
  }
}
</style>
