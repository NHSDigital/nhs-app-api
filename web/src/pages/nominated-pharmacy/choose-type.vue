<template>
  <div v-if="showTemplate">

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="showErrors" message-type="error">
          <message-text id="errorHeading" data-purpose="error-heading">
            {{ $t('nominated_pharmacy.chooseType.errorHeading') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('nominated_pharmacy.chooseType.errorMessage') }}</li>
          </message-list>
        </message-dialog>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <radio-group v-model="selectedValue"
                     :class="$style.radioGroup"
                     :radios="radioButtons"
                     :show-error="showErrors"
                     :current-value="currentChoice"
                     :error-message="$t('nominated_pharmacy.chooseType.errorMessage')"
                     @select="selected"/>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <generic-button id="continue-button"
                        :class="['nhsuk-button']"
                        @click.prevent="continueClicked">
          {{ $t('nominated_pharmacy.chooseType.buttonText') }}
        </generic-button>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink id="back-link"
                                  :path="interruptPath"
                                  :button-text="'nominated_pharmacy.chooseType.backLinkText'"
                                  @clickAndPrevent="backLinkClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RadioGroup from '@/components/RadioGroup';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_INTERRUPT, NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES, PRESCRIPTIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    AnalyticsTrackedTag,
    DesktopGenericBackLink,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    RadioGroup,
  },
  data() {
    return {
      interruptPath: NOMINATED_PHARMACY_INTERRUPT.path,
      highStreetSearchPath: NOMINATED_PHARMACY_SEARCH.path,
      onlineOnlyChoicesPath: NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path,
      hasTriedToContinue: false,
      radioButtons: [
        {
          hint: this.$t('nominated_pharmacy.chooseType.highStreetHint'),
          label: this.$t('nominated_pharmacy.chooseType.highStreet'),
          value: PharmacyTypeChoice.HIGH_STREET_PHARMACY,
        },
        {
          hint: this.$t('nominated_pharmacy.chooseType.onlineHint'),
          label: this.$t('nominated_pharmacy.chooseType.online'),
          value: PharmacyTypeChoice.ONLINE_PHARMACY,
        },
      ],
      selectedValue: this.$store.state.nominatedPharmacy.chosenType,
    };
  },
  computed: {
    currentChoice() {
      return get('$store.state.nominatedPharmacy.chosenType')(this);
    },
    hasMadeDecision() {
      return this.selectedValue !== null;
    },
    showErrors() {
      return this.hasTriedToContinue && !this.hasMadeDecision;
    },
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path);
    }
  },
  methods: {
    continueClicked() {
      this.hasTriedToContinue = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }

      this.$store.dispatch('nominatedPharmacy/setChosenType', this.selectedValue);
      if (this.selectedValue === PharmacyTypeChoice.HIGH_STREET_PHARMACY) {
        redirectTo(this, this.highStreetSearchPath);
      } else {
        redirectTo(this, this.onlineOnlyChoicesPath);
      }
    },
    selected(value) {
      this.selectedValue = value;
    },
    backLinkClicked() {
      redirectTo(this, this.interruptPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";
@import "../../style/spacings";
</style>
