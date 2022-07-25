<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <form-error-summary v-if="showErrors"
                          :header-locale-ref="'organDonation.thereIsAProblem'"
                          :errors="$t('organDonation.yourChoice.chooseToDonate')"
                          :errors-ids="'choice-' + getFirstChoiceValue('radioButtons')"/>

      <div>
        <h2>{{ $t('organDonation.yourChoice.yourChoice') }}</h2>
      </div>
      <nhs-uk-radio-group v-model="selectedValue"
                          name="choice"
                          :items="radioButtons"
                          :current-value="currentChoice"
                          :legend-size="'xs'"
                          :heading="$t('organDonation.yourChoice.youCanDonateSomeOrAll')"
                          :required="true"
                          :error="showErrors"
                          :error-text="$t('organDonation.yourChoice.chooseToDonate')"
                          @onselect="selected"
      />
      <generic-button id="continue-button"
                      :class="['nhsuk-button']"
                      @click.prevent="continueClicked">
        {{ $t('generic.continue') }}
      </generic-button>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="backLink"
                                 :button-text="'generic.back'"
                                 @clickAndPrevent="backClicked"/>
    </div>
  </div>
</template>
<script>
import get from 'lodash/fp/get';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import isNil from 'lodash/fp/isNil';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import DynamicBackLinkMixin from '@/components/organ-donation/DynamicBackLinkMixin';
import FormErrorSummary from '@/components/FormErrorSummary';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import { EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import {
  ORGAN_DONATION_FAITH_PATH,
  ORGAN_DONATION_SOME_ORGANS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

export default {
  components: {
    GenericButton,
    DesktopGenericBackLink,
    FormErrorSummary,
    NhsUkRadioGroup,
  },
  mixins: [DynamicBackLinkMixin, EnsureOptInDecision],
  data() {
    return {
      hasTriedToContinue: false,
      radioButtons: [
        {
          label: this.$t('organDonation.yourChoice.allMyOrgansAndTissue'),
          value: true,
          hint: {
            text: this.$t('organDonation.yourChoice.helpUpToNinePeople'),
          },
        },
        {
          label: this.$t('organDonation.yourChoice.someOrgansAndTissue'),
          value: false,
          hint: {
            text: this.$t('organDonation.yourChoice.chooseWhichOrgansAndTissue'),
          },
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
    continueClicked() {
      this.hasTriedToContinue = false;

      this.$nextTick(() => {
        this.hasTriedToContinue = true;

        if (this.showErrors) {
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
          return;
        }

        if (this.currentChoice) {
          redirectTo(this, ORGAN_DONATION_FAITH_PATH);
        } else {
          redirectTo(this, ORGAN_DONATION_SOME_ORGANS_PATH);
        }
      });
    },
    selected(value) {
      this.$store.dispatch(this.setAllOrgansAction, value);
    },
    backButtonClicked() {
      this.hasTriedToContinue = false;
      this.backClicked();
    },
    getFirstChoiceValue(choicesName) {
      if (get(`${choicesName}[0].value`, this) !== undefined && get(`${choicesName}[0].value`, this) !== '') {
        return get(`${choicesName}[0].value`, this);
      }
      if (get(`${choicesName}[0].code`, this) !== undefined && get(`${choicesName}[0].code`, this) !== '') {
        return get(`${choicesName}[0].code`, this);
      }
      return '';
    },
  },
};
</script>
