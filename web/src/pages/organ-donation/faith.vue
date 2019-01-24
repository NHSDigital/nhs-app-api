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
    <generic-button id="back-to-your-choice"
                    :class="[$style.button, $style.grey]"
                    @click.stop.prevent="goBack">
      {{ $t('organDonation.faith.backButtonText') }}
    </generic-button>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import GenericButton from '@/components/widgets/GenericButton';
import { YES, NO, NOT_STATED } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_YOUR_CHOICE, ORGAN_DONATION_ADDITIONAL_DETAILS } from '@/lib/routes';
import { EnsureAllOrgansDecision } from '@/components/organ-donation/EnsureDecisionMixin';

export default {
  components: {
    GenericRadioButton,
    GenericButton,
    MessageDialog,
    MessageText,
    MessageList,
  },
  mixins: [EnsureAllOrgansDecision],
  data() {
    return {
      yesValue: YES,
      noValue: NO,
      preferNotToSayValue: NOT_STATED,
      hasTriedToContinue: false,
    };
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
    goBack() {
      this.$router.push(ORGAN_DONATION_YOUR_CHOICE.path);
    },
    continueClicked() {
      this.hasTriedToContinue = true;
      if (this.showError) {
        window.scrollTo(0, 0);
      } else {
        this.$router.push(ORGAN_DONATION_ADDITIONAL_DETAILS.path);
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
