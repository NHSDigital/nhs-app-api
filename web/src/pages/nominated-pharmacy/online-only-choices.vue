<template>
  <div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="showErrors" id="error-message" message-type="error" :focusable="true">
          <message-text data-purpose="error-heading">
            {{ $t('nominatedPharmacy.onlineOnlyChoices.errorMessageHeader') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('nominatedPharmacy.onlineOnlyChoices.errorMessageText') }}</li>
          </message-list>
        </message-dialog>
        <radio-group v-model="onlineOnlyChoice"
                     class="nhsuk-u-padding-top-2"
                     :radios="radioButtons"
                     :show-error="showErrors"
                     :current-value="currentChoice"
                     :error-message="$t('nominatedPharmacy.onlineOnlyChoices.errorMessageText')"
                     @select="selected"/>
        <generic-button id="continue-button"
                        class="nhsuk-button nhsuk-u-padding-top-2"
                        @click.prevent="continueClicked">
          {{ $t('nominatedPharmacy.onlineOnlyChoices.continueButton') }}
        </generic-button>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.back')">
          <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                  id="back-link"
                                  :path="dspInterrupt"
                                  :button-text="'generic.back'"
                                  @clickAndPrevent="backButtonClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>


<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RadioGroup from '@/components/RadioGroup';
import { redirectTo } from '@/lib/utils';
import {
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_PATH,
  NOMINATED_PHARMACY_SEARCH_RESULTS_PATH,
  NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  PRESCRIPTIONS_PATH,
} from '@/router/paths';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';

export default {
  layout: 'nhsuk-layout',
  name: 'OnlineOnlyChoices',
  components: {
    RadioGroup,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    DesktopGenericBackLink,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      dspInterrupt: NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
      hasTriedToContinue: false,
      onlineOnlyChoice: this.$store.getters['nominatedPharmacy/getOnlineOnlyKnownOption'],
      radioButtons: [
        {
          label: this.$t('nominatedPharmacy.onlineOnlyChoices.yesLabel'),
          value: true,
        },
        {
          label: this.$t('nominatedPharmacy.onlineOnlyChoices.noLabel'),
          value: false,
        },
      ],
    };
  },
  computed: {
    hasMadeDecision() {
      return this.onlineOnlyChoice !== null;
    },
    showErrors() {
      return this.hasTriedToContinue && !this.hasMadeDecision;
    },
    currentChoice() {
      return this.$store.getters['nominatedPharmacy/getOnlineOnlyKnownOption'];
    },
  },
  created() {
    redirectTo(this, PRESCRIPTIONS_PATH);
  },
  methods: {
    async continueClicked() {
      this.hasTriedToContinue = true;

      if (this.showErrors) {
        EventBus.$emit(FOCUS_ERROR_ELEMENT);
        return;
      }

      this.$store.dispatch('nominatedPharmacy/setOnlineOnlyKnownOption', this.onlineOnlyChoice);

      if (this.onlineOnlyChoice === true) {
        redirectTo(this, NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_PATH);
      } else {
        const pharmacySearchResponse = await this.getRandomOnlinePharmacies();

        if (pharmacySearchResponse.technicalError) {
          this.submissionError = true;
          return;
        }

        this.$store.dispatch('nominatedPharmacy/setSearchQuery', '');
        this.$store.dispatch('nominatedPharmacy/setSearchResults', pharmacySearchResponse);

        redirectTo(this, NOMINATED_PHARMACY_SEARCH_RESULTS_PATH);
      }
    },
    async getRandomOnlinePharmacies() {
      const pharmacySearchResult = {
        noResultsFound: false,
        technicalError: false,
        pharmacies: [],
      };

      try {
        const response = await this.$store.app.$http.getV1PatientOnlinePharmacies();
        pharmacySearchResult.pharmacies = response.pharmacies;
        pharmacySearchResult.noResultsFound = pharmacySearchResult.pharmacies.length === 0;
      } catch {
        pharmacySearchResult.technicalError = true;
      }

      return pharmacySearchResult;
    },
    selected(value) {
      this.onlineOnlyChoice = value;
    },
    backButtonClicked() {
      redirectTo(this, this.dspInterrupt);
    },
  },
};
</script>

<style scoped>
</style>
