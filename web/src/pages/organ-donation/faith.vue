<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <message-dialog v-if="showError" message-type="error" :focusable="true">
        <message-text data-purpose="error-heading">
          {{ $t('organDonation.thereIsAProblem') }}
        </message-text>
        <message-list data-purpose="reason-error">
          <li>{{ $t('organDonation.faith.respondToFaithBeliefDeclaration') }}</li>
        </message-list>
      </message-dialog>
      <h2>{{ $t('organDonation.faith.faithSlashBeliefs') }}</h2>
      <p>{{ $t('organDonation.faith.askFamilyWhenYouDie') }}</p>
      <collapsible-dialog>
        <template slot="header">
          {{ $t('organDonation.faith.endOfLifeWishes.examplesOfEndOfLifeWishes') }}
        </template>
        <ul class="nhsuk-list">
          <li v-for="(listItem, index) of $t('organDonation.faith.endOfLifeWishes.examples')"
              :key="index">
            {{ listItem }}
          </li>
        </ul>
      </collapsible-dialog>
      <p>{{ $t('organDonation.faith.recordWhetherToAskFamilyWhenYouDie') }}</p>
      <p><strong>{{ $t('organDonation.faith.iWouldLikeStaffToSpeakToMyFamily') }}</strong></p>
      <radio-group v-model="selectedValue"
                   :radios="choices"
                   :current-value="currentChoice"
                   :show-error="showError"
                   :error-message="$t('organDonation.faith.chooseYesNoOrPreferNot')"
                   @select="radioButtonSelected"/>
      <generic-button id="continue-to-additional-details"
                      :class="['nhsuk-button']"
                      @click.stop.prevent="continueClicked">
        {{ $t('generic.continue') }}
      </generic-button>
      <back-button v-if="!$store.state.device.isNativeApp"/>
    </div>
  </div>
</template>

<script>
import BackButton from '@/components/BackButton';
import CollapsibleDialog from '@/components/widgets/collapsible/CollapsibleDialog';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import RadioGroup from '@/components/RadioGroup';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import { NO, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_ADDITIONAL_DETAILS_PATH } from '@/router/paths';
import { EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

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
        { value: YES, label: this.$t('organDonation.faith.yesThisIsApplicable') },
        { value: NO, label: this.$t('organDonation.faith.noThisIsNotApplicable') },
        { value: NOT_STATED, label: this.$t('organDonation.faith.preferNotToSay') },
      ],
      hasTriedToContinue: false,
      selectedValue: undefined,
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
  beforeMount() {
    if (
      this.$store.state.organDonation.isAmending &&
      isDefault({
        path: 'registration.faithDeclaration',
        state: this.$store.state.organDonation,
      })
    ) {
      this.$store.dispatch('organDonation/cloneFromOriginal', 'faithDeclaration');
    }
  },
  methods: {
    radioButtonSelected(value) {
      this.$store.dispatch('organDonation/setFaithDeclaration', value);
    },
    continueClicked() {
      this.hasTriedToContinue = true;
      if (this.showError) {
        EventBus.$emit(FOCUS_ERROR_ELEMENT);
      } else {
        redirectTo(this, ORGAN_DONATION_ADDITIONAL_DETAILS_PATH);
      }
    },
  },
};
</script>
