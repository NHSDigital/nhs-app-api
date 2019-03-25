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
      <p>{{ $t('organDonation.faith.body.paragraph1') }}</p>
    </div>
    <collapsible-dialog>
      <template slot="header">
        {{ $t('organDonation.faith.endOfLifeWishes.header') }}
      </template>
      <ul>
        <li v-for="(listItem, index) of $t('organDonation.faith.endOfLifeWishes.listItems')"
            :key="index">
          {{ listItem }}
        </li>
      </ul>
    </collapsible-dialog>
    <div :class="$style.info">
      <p>{{ $t('organDonation.faith.body.paragraph2') }}</p>
    </div>
    <div :class="$style.info">
      <p><b>{{ $t('organDonation.faith.choices.header') }}</b></p>
    </div>
    <radio-group :radios="choices"
                 :current-value="currentChoice"
                 :show-error="showError"
                 :error-message="$t('organDonation.faith.inlineErrorMessage')"
                 @select="radioButtonSelected"/>
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
import CollapsibleDialog from '@/components/widgets/CollapsibleDialog';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import RadioGroup from '@/components/RadioGroup';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import { NO, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_ADDITIONAL_DETAILS } from '@/lib/routes';
import { EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    BackButton,
    CollapsibleDialog,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    RadioGroup,
  },
  mixins: [EnsureOptInDecision],
  data() {
    return {
      choices: [
        { value: YES, label: this.$t('organDonation.faith.choices.yes.title') },
        { value: NO, label: this.$t('organDonation.faith.choices.no.title') },
        { value: NOT_STATED, label: this.$t('organDonation.faith.choices.preferNotToSay.title') },
      ],
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
ul {
  list-style-type: none;
  margin: 0;
  padding: 0;
  margin-bottom: 1em;
  li {
    margin: 0;
    padding: 0;
    line-height:1em;
  }
}

</style>
