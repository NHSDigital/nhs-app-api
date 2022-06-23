<template>
  <div v-if="showTemplate">
    <!-- NB Following div ensures legend focus within radio group announced - NHSO-17263 -->
    <div class="nhsuk-u-visually-hidden" role="status" tabindex="-1"/>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <error-dialog v-if="showErrors"
                      :header-locale-ref="'nominatedPharmacy.chooseType.errorHeading'"
                      :errors="$t('nominatedPharmacy.chooseType.errorMessage')"/>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <nhs-uk-radio-group
          v-model="selectedValue"
          name="chooseType"
          :legend-size="'xl'"
          :heading="'<h1>'
            + $t('nominatedPharmacy.chooseType.nominatedPharmacyChooseType')
            + '</h1>'"
          :heading-as-html="true"
          :error="showErrors"
          :error-text="$t('nominatedPharmacy.chooseType.errorMessage')"
          :items="radioButtons"
          :current-value="currentChoice"
          @selected="selected"
        />
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <generic-button id="continue-button"
                        :class="['nhsuk-button']"
                        @click.prevent="continueClicked">
          {{ $t('nominatedPharmacy.chooseType.buttonText') }}
        </generic-button>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag v-if="!$store.state.device.isNativeApp"
                               :text="$t('generic.back')">
          <desktop-generic-back-link id="back-link"
                                     :path="interruptPath"
                                     :button-text="'nominatedPharmacy.chooseType.backLinkText'"/>
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
import ErrorDialog from '@/components/ErrorDialog';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import {
  NOMINATED_PHARMACY_SEARCH_PATH,
  NOMINATED_PHARMACY_INTERRUPT_PATH,
  NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  PRESCRIPTIONS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';

export default {
  components: {
    AnalyticsTrackedTag,
    DesktopGenericBackLink,
    GenericButton,
    ErrorDialog,
    NhsUkRadioGroup,
  },
  layout: 'nhsuk-layout',
  data() {
    return {
      interruptPath: NOMINATED_PHARMACY_INTERRUPT_PATH,
      highStreetSearchPath: NOMINATED_PHARMACY_SEARCH_PATH,
      dspInterruptPath: NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
      hasContinuedWithoutSelection: false,
      radioButtons: [
        {
          hint: { text: this.$t('nominatedPharmacy.chooseType.highStreetHint') },
          label: this.$t('nominatedPharmacy.chooseType.highStreet'),
          value: PharmacyTypeChoice.HIGH_STREET_PHARMACY,
        },
        {
          hint: { text: this.$t('nominatedPharmacy.chooseType.onlineHint') },
          label: this.$t('nominatedPharmacy.chooseType.online'),
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
    showErrors() {
      return this.hasContinuedWithoutSelection;
    },
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS_PATH);
    }
  },
  methods: {
    continueClicked() {
      this.hasContinuedWithoutSelection = false;
      if (this.selectedValue === null) {
        this.$nextTick(() => {
          this.hasContinuedWithoutSelection = true;
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
        });
        return;
      }

      this.$store.dispatch('nominatedPharmacy/setChosenType', this.selectedValue);
      if (this.selectedValue === PharmacyTypeChoice.HIGH_STREET_PHARMACY) {
        redirectTo(this, this.highStreetSearchPath);
      } else {
        redirectTo(this, this.dspInterruptPath);
      }
    },
    selected(value) {
      this.selectedValue = value;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/buttons";
@import "../../style/spacings";
</style>
