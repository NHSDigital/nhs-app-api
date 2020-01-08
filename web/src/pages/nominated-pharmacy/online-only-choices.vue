<template>
  <div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="showErrors" id="error-message" message-type="error">
          <message-text data-purpose="error-heading">
            {{ $t('nominatedPharmacyOnlineOnlyChoices.errorMessageHeader') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('nominatedPharmacyOnlineOnlyChoices.errorMessageText') }}</li>
          </message-list>
        </message-dialog>
        <radio-group v-model="onlineOnlyChoice"
                     class="nhsuk-u-padding-top-2"
                     :radios="radioButtons"
                     :show-error="showErrors"
                     :current-value="currentChoice"
                     :error-message="$t('nominatedPharmacyOnlineOnlyChoices.errorMessageText')"
                     @select="selected"/>
        <generic-button id="continue-button"
                        class="nhsuk-button nhsuk-u-padding-top-2"
                        @click.prevent="continueClicked">
          {{ $t('nominatedPharmacyOnlineOnlyChoices.continueButton') }}
        </generic-button>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                  id="back-link"
                                  :path="nominatedPharmacyInterruptPath"
                                  :button-text="'generic.backButton.text'"
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
import { NOMINATED_PHARMACY_INTERRUPT, NOMINATED_PHARMACY_CHOOSE_TYPE } from '@/lib/routes';

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
      nominatedPharmacyChooseType: NOMINATED_PHARMACY_CHOOSE_TYPE.path,
      nominatedPharmacyInterruptPath: NOMINATED_PHARMACY_INTERRUPT.path,
      hasTriedToContinue: false,
      onlineOnlyChoice: undefined,
      radioButtons: [
        {
          label: this.$t('nominatedPharmacyOnlineOnlyChoices.yesLabel'),
          value: true,
        },
        {
          label: this.$t('nominatedPharmacyOnlineOnlyChoices.noLabel'),
          value: false,
        },
      ],
    };
  },
  computed: {
    hasMadeDecision() {
      return !(this.onlineOnlyChoice === '' || this.onlineOnlyChoice === undefined);
    },
    showErrors() {
      return this.hasTriedToContinue && !this.hasMadeDecision;
    },
    currentChoice() {
      return this.$store.getters['nominatedPharmacy/getOnlineOnlyKnownOption'];
    },
  },
  methods: {
    continueClicked() {
      this.hasTriedToContinue = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }
      this.$store.dispatch('nominatedPharmacy/setOnlineOnlyKnownOption', this.onlineOnlyChoice);

      if (this.onlineOnlyChoice === true) {
        // todo: update with path to correct page
        redirectTo(this, NOMINATED_PHARMACY_INTERRUPT.path);
      } else {
        // todo: update with path to correct page
        redirectTo(this, NOMINATED_PHARMACY_INTERRUPT.path);
      }
    },
    selected(value) {
      this.onlineOnlyChoice = value;
    },
    backButtonClicked() {
      redirectTo(this, this.nominatedPharmacyChooseType);
    },
  },
};
</script>

<style scoped>

</style>
