<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div ref="validationMessage" tabindex="-1">
      <message-dialog v-if="showErrors" message-type="error">
        <message-text data-purpose="error-heading">
          {{ $t('organDonation.someOrgans.errorMsgHeader') }}
        </message-text>
        <message-list data-purpose="reason-error">
          <li v-if="!areAllSelected">
            {{ $t('organDonation.someOrgans.allSelectedValidationText') }}
          </li>
          <li v-if="areAllSelected && !hasYesSelection">
            {{ $t('organDonation.someOrgans.yesRequiredValidationText') }}
          </li>
        </message-list>
      </message-dialog>
    </div>
    <div :class="$style.info">
      <h2>{{ $t('organDonation.someOrgans.subheader') }}</h2>
      <p>{{ $t('organDonation.someOrgans.description') }}</p>
    </div>
    <organ-choice v-for="choice in choices" :key="choice"
                  :title="`organDonation.someOrgans.${choice}Title`"
                  :organ-name="choice"
                  :show-errors="showInlineErrors"/>
    <generic-button id="continue-button"
                    :class="[$style.button, $style.green]"
                    @click.prevent="continueClicked">
      {{ $t('organDonation.someOrgans.continueButtonText') }}
    </generic-button>
    <back-button />
  </div>
</template>
<script>
/* eslint-disable no-restricted-syntax */
import BackButton from '@/components/BackButton';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import includes from 'lodash/fp/includes';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import OrganChoice from '@/components/organ-donation/OrganChoice';
import { initialState, NO, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import { ORGAN_DONATION_FAITH } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  scrollToTop: true,
  components: {
    BackButton,
    ErrorMessage,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    OrganChoice,
  },
  mixins: [EnsureDecisionMixin],
  data() {
    return {
      activeErrorMessage: '',
      choices: Object.keys(initialState().registration.decisionDetails.choices),
      hasTriedToContinue: false,
      setAllOrganChoice: 'organDonation/setSomeOrgans',
    };
  },
  computed: {
    areAllSelected() {
      return !includes(NOT_STATED)(this.currentChoices);
    },
    currentChoices() {
      return this.$store.state.organDonation.registration.decisionDetails.choices;
    },
    hasMadeChoices() {
      return this.areAllSelected && this.hasYesSelection;
    },
    hasNoSelection() {
      return includes(NO)(this.currentChoices);
    },
    hasYesSelection() {
      return includes(YES)(this.currentChoices);
    },
    showErrors() {
      return this.hasTriedToContinue && !this.hasMadeChoices;
    },
    showInlineErrors() {
      return this.showErrors && !this.areAllSelected &&
        (this.hasYesSelection || this.hasNoSelection);
    },
  },
  asyncData({ store }) {
    if (
      (store.state.organDonation.isAmending || store.state.organDonation.isReaffirming) &&
      isDefault({
        path: 'registration.decisionDetails.choices',
        state: store.state.organDonation,
      })
    ) {
      store.dispatch('organDonation/cloneFromOriginal', 'decisionDetails.choices');
    }
  },
  methods: {
    continueClicked() {
      this.hasTriedToContinue = true;
      if (this.hasMadeChoices) {
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
</style>
