<template>
  <terms-and-conditions-layout>
    <div v-if="showTemplate">
      <message-dialog v-if="showError"
                      :focusable="true"
                      message-type="error"
                      role="alert"
                      tabindex="-1">
        <message-text data-purpose="error-heading">
          {{ $t('userResearch.thereIsAProblem') }}
        </message-text>
        <message-list data-purpose="reason-error">
          <li>
            {{ $t('userResearch.selectYesOrNo') }}
          </li>
        </message-list>
      </message-dialog>
      <p>{{ $t('userResearch.weWouldLikeToContactYouAboutUserResearch') }}</p>
      <collapsible-details>
        <template slot="header">
          {{ $t('userResearch.whatIsInvolved') }}
        </template>
        <p>{{ $t('userResearch.weWillAddYouToOurPanel') }}</p>
        <p>{{ $t('userResearch.youMightBeAskedTo') }}</p>
        <ul>
          <li>{{ $t('userResearch.tryOutNewFeatures') }}</li>
          <li>{{ $t('userResearch.answerMoreQuestionsByEmail') }}</li>
          <li>{{ $t('userResearch.talkToOurResearchers') }}</li>
        </ul>
        <p>{{ $t('userResearch.youCanSayNoAndLeaveAtAnyTime') }}</p>
      </collapsible-details>
      <p>
        {{ $t('userResearch.yourInformationWillOnlyBeUsedForThePanel')
        }}<a class="inline"
             :href="privacyPolicyUrl"
             target="_blank"
             rel="noopener noreferrer">{{
               $t('userResearch.readOurPrivacyPolicy') }}</a>{{
          $t('userResearch.toFindOutHowWeUseAndProtectYourData') }}
      </p>
      <radio-group :header="$t('userResearch.canWeContactYouToTakePart')"
                   header-size="m"
                   class="nhsuk-u-margin-bottom-4"
                   :show-error="showError"
                   :error-message="$t('userResearch.selectYesOrNo')"
                   :radios="choices"
                   @select="onSelection"/>
      <primary-button @click.stop.prevent="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </terms-and-conditions-layout>
</template>

<script>
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import PrimaryButton from '@/components/PrimaryButton';
import RadioGroup from '@/components/RadioGroup';
import TermsConditionsMixin from '@/components/TermsConditionsMixin';
import TermsAndConditionsLayout from '@/layouts/termsAndConditions';
import isUndefined from 'lodash/fp/isUndefined';
import {
  PRIVACY_POLICY_URL,
} from '@/router/externalLinks';

export default {
  name: 'UserResearchPage',
  components: {
    CollapsibleDetails,
    MessageDialog,
    MessageList,
    MessageText,
    PrimaryButton,
    RadioGroup,
    TermsAndConditionsLayout,
  },
  mixins: [TermsConditionsMixin],
  data() {
    return {
      choices: [
        { label: this.$t('userResearch.yesYouCanContactMe'), value: 'optIn' },
        { label: this.$t('userResearch.noDoNotContactMe'), value: 'optOut' },
      ],
      hasTriedToContinue: false,
      privacyPolicyUrl: PRIVACY_POLICY_URL,
      selectedValue: undefined,
    };
  },
  computed: {
    showError() {
      return this.hasTriedToContinue && isUndefined(this.selectedValue);
    },
  },
  methods: {
    async onContinue() {
      this.hasTriedToContinue = true;

      if (!this.showError) {
        try {
          await this.$store.app.$http.postV1ApiUsersMeInfoUserresearch({
            userResearchRequest: { preference: this.selectedValue },
            ignoreError: true,
          });
        } catch {
          // do nothing
        } finally {
          this.conditionalRedirect();
        }
      }
    },
    onSelection(value) {
      this.selectedValue = value;
    },
  },
};
</script>
