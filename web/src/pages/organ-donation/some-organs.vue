<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
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
      <div>
        <h2>{{ $t('organDonation.someOrgans.subheader') }}</h2>
        <nhs-arrow-banner :banner-text="$t('organDonation.someOrgans.moreAboutOrgansLinkText')"
                          :open-new-window="false"
                          :click-action="moreAboutOrgansClicked" />
      </div>
      <hr class="nhsuk-section-break nhsuk-section-break--m" aria-hidden="true">
      <div>
        <p><strong>{{ $t('organDonation.someOrgans.choices.subheader') }}</strong></p>
      </div>
      <organ-choice v-for="choice in choices" :key="choice"
                    :title="`organDonation.someOrgans.choices.${choice}Title`"
                    :organ-name="choice"
                    :show-errors="showInlineErrors"/>
      <generic-button id="continue-button"
                      :class="['nhsuk-button']"
                      @click.prevent="continueClicked">
        {{ $t('organDonation.someOrgans.continueButtonText') }}
      </generic-button>
      <back-button v-if="!$store.state.device.isNativeApp"/>
    </div>
  </div>
</template>
<script>
/* eslint-disable no-restricted-syntax */
import includes from 'lodash/fp/includes';
import BackButton from '@/components/BackButton';
import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import OrganChoice from '@/components/organ-donation/OrganChoice';
import { initialState, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';
import { isDefault } from '@/lib/organ-donation/registration-comparison';
import { ORGAN_DONATION_FAITH, ORGAN_DONATION_MORE_ABOUT_ORGANS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  scrollToTop: true,
  components: {
    BackButton,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    NhsArrowBanner,
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
    hasYesSelection() {
      return includes(YES)(this.currentChoices);
    },
    showErrors() {
      return this.hasTriedToContinue && !this.hasMadeChoices;
    },
    showInlineErrors() {
      return this.showErrors && !this.areAllSelected;
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
        redirectTo(this, ORGAN_DONATION_FAITH.path);
        return;
      }
      window.scrollTo(0, 0);
    },
    moreAboutOrgansClicked() {
      this.$router.push(ORGAN_DONATION_MORE_ABOUT_ORGANS.path);
    },
  },
};
</script>
