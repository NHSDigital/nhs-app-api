<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <message-dialog v-if="showErrors" message-type="error">
        <message-text data-purpose="error-heading">
          {{ $t('organDonation.thereIsAProblem') }}
        </message-text>
        <message-list data-purpose="reason-error">
          <li>{{ $t('organDonation.yourChoice.chooseToDonate') }}</li>
        </message-list>
      </message-dialog>
      <div>
        <h2>{{ $t('organDonation.yourChoice.yourChoice') }}</h2>
        <p>{{ $t('organDonation.yourChoice.youCanDonateSomeOrAll') }}</p>
      </div>
      <radio-group v-model="selectedValue"
                   :current-value="currentChoice"
                   :radios="radioButtons"
                   :show-error="showErrors"
                   :error-message="$t('organDonation.yourChoice.chooseToDonate')"
                   @select="selected"/>
      <generic-button id="continue-button"
                      :class="['nhsuk-button']"
                      @click.prevent="continueClicked">
        {{ $t('generic.continue') }}
      </generic-button>
      <back-button v-if="!$store.state.device.isNativeApp" :before="beforeBack" />
    </div>
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
  ORGAN_DONATION_FAITH_PATH,
  ORGAN_DONATION_SOME_ORGANS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

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
          hint: this.$t('organDonation.yourChoice.helpUpToNinePeople'),
          label: this.$t('organDonation.yourChoice.allMyOrgansAndTissue'),
          value: true,
        },
        {
          hint: this.$t('organDonation.yourChoice.chooseWhichOrgansAndTissue'),
          label: this.$t('organDonation.yourChoice.someOrgansAndTissue'),
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
  beforeMount() {
    if (
      this.$store.state.organDonation.isAmending &&
      isDefault({
        path: 'registration.decisionDetails.all',
        state: this.$store.state.organDonation,
      })
    ) {
      this.$store.dispatch('organDonation/cloneFromOriginal', 'decisionDetails.all');
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

      if (this.currentChoice) {
        redirectTo(this, ORGAN_DONATION_FAITH_PATH);
      } else {
        redirectTo(this, ORGAN_DONATION_SOME_ORGANS_PATH);
      }
    },
    selected(value) {
      this.$store.dispatch(this.setAllOrgansAction, value);
    },
  },
};
</script>
