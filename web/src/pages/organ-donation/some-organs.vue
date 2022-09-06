<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div ref="validationMessage" tabindex="-1">
        <form-error-summary v-if="showErrors"
                            :header-locale-ref="'organDonation.thereIsAProblem'"
                            :errors="errors"
                            :errors-ids="errorIds"/>
      </div>
      <div>
        <h2>{{ $t('organDonation.someOrgans.yourChoice') }}</h2>
        <nhs-arrow-banner
          :banner-text="$t('organDonation.someOrgans.findOutMoreAboutOrgansAndTissue')"
          :open-new-window="false"
          :click-action="moreAboutOrgansClicked" />
      </div>
      <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      <div>
        <p><strong>
          {{ $t('organDonation.someOrgans.pleaseSelectWhichYouWouldLikeToDonate') }}</strong></p>
      </div>
      <organ-choice v-for="choice in choices" :key="choice"
                    :title="`organDonation.organs.${choice}`"
                    :organ-name="choice"
                    :show-error="showInlineErrors && isNotStated(choice)"/>
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
/* eslint-disable no-restricted-syntax */
import includes from 'lodash/fp/includes';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import DynamicBackLinkMixin from '@/components/organ-donation/DynamicBackLinkMixin';
import FormErrorSummary from '@/components/FormErrorSummary';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import OrganChoice from '@/components/organ-donation/OrganChoice';
import { initialState, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import { ORGAN_DONATION_FAITH_PATH, ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

export default {
  components: {
    GenericButton,
    DesktopGenericBackLink,
    FormErrorSummary,
    NhsArrowBanner,
    OrganChoice,
  },
  mixins: [EnsureDecisionMixin, DynamicBackLinkMixin],
  scrollToTop: true,
  data() {
    return {
      activeErrorMessage: '',
      choices: Object.keys(initialState().registration.decisionDetails.choices),
      hasTriedToContinue: false,
      hasMadeChoices: false,
      showInlineErrors: false,
      decisionsAfterContinue: [],
      errors: [],
      errorIds: [],
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
    hasYesSelection() {
      return includes(YES)(this.currentChoices);
    },
    showErrors() {
      const errors = this.getErrors;
      return this.hasTriedToContinue && (!this.hasMadeChoices || errors.length > 0);
    },
    getErrors() {
      const errors = [];

      if (!this.areAllSelected) {
        const organs = Object.keys(this.$store.state.organDonation.registration.decisionDetails.choices);
        const decisions = Object.values(this.$store.state.organDonation.registration.decisionDetails.choices);

        for (let i = 0; i < organs.length; i += 1) {
          if (decisions[i] === NOT_STATED) {
            const organName = this.$t(`organDonation.organs.${organs[i]}`);
            errors.push(this.$t('organDonation.someOrgans.chooseYesOrNoFor') + organName.toLowerCase());
          }
        }
      }

      if (this.areAllSelected && !this.hasYesSelection) {
        errors.push(this.$t('organDonation.someOrgans.chooseYesForAtLeastOneOrgan'));
      }

      return errors;
    },
    getErrorIds() {
      const errorsIds = [];

      if (!this.areAllSelected) {
        const organs = Object.keys(this.$store.state.organDonation.registration.decisionDetails.choices);
        const decisions = Object.values(this.$store.state.organDonation.registration.decisionDetails.choices);

        for (let i = 0; i < organs.length; i += 1) {
          if (decisions[i] === NOT_STATED) {
            errorsIds.push(`${organs[i]}-Yes`);
          }
        }
      }

      if (this.areAllSelected && !this.hasYesSelection) {
        errorsIds.push(`${this.choices[0]}-Yes`);
      }

      return errorsIds;
    },
  },
  beforeMount() {
    if (
      (this.$store.state.organDonation.isAmending ||
        this.$store.state.organDonation.isReaffirming) &&
      isDefault({
        path: 'registration.decisionDetails.choices',
        state: this.$store.state.organDonation,
      })
    ) {
      this.$store.dispatch('organDonation/cloneFromOriginal', 'decisionDetails.choices');
    }
  },
  methods: {
    continueClicked() {
      this.hasTriedToContinue = false;

      this.$nextTick(() => {
        this.hasTriedToContinue = true;
        this.hasMadeChoices = this.areAllSelected && this.hasYesSelection;
        this.showInlineErrors = this.showErrors && !this.areAllSelected;
        this.errors = this.getErrors;
        this.errorIds = this.getErrorIds;
        this.decisionsAfterContinue = Object.values(this.$store.state.organDonation.registration.decisionDetails.choices);

        if (this.showErrors) {
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
          return;
        }
        redirectTo(this, ORGAN_DONATION_FAITH_PATH);
      });
    },
    moreAboutOrgansClicked() {
      redirectTo(this, ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH);
    },
    isNotStated(organName) {
      const organs = Object.keys(this.$store.state.organDonation.registration.decisionDetails.choices);

      for (let i = 0; i < organs.length; i += 1) {
        if (organs[i] === organName) {
          return this.decisionsAfterContinue[i] === NOT_STATED;
        }
      }

      return false;
    },
  },
};
</script>
