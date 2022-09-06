<template>
  <terms-and-conditions-layout>
    <div v-if="showTemplate">
      <form-error-summary v-if="showError"
                          :header-locale-ref="'userResearch.thereIsAProblem'"
                          :errors="$t('userResearch.selectYesOrNo')"
                          :errors-ids="`userResearch-${getFirstChoiceValue('choices')}`"/>

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
      <nhs-uk-radio-group
        v-model="selectedValue"
        name="userResearch"
        :legend-size="'m'"
        :heading="$t('userResearch.canWeContactYouToTakePart')"
        :error="showError"
        :error-heading-reference="'userResearch.thereIsAProblem'"
        :error-text="$t('userResearch.selectYesOrNo')"
        :items="choices"
      />
      <primary-button @click.stop.prevent="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </terms-and-conditions-layout>
</template>

<script>
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';
import PrimaryButton from '@/components/PrimaryButton';
import FormErrorSummary from '@/components/FormErrorSummary';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import TermsAndConditionsLayout from '@/layouts/termsAndConditions';
import isUndefined from 'lodash/fp/isUndefined';
import { NOTIFICATIONS_PATH } from '@/router/paths';
import get from 'lodash/fp/get';

export default {
  name: 'UserResearchPage',
  components: {
    CollapsibleDetails,
    PrimaryButton,
    TermsAndConditionsLayout,
    NhsUkRadioGroup,
    FormErrorSummary,
  },
  data() {
    return {
      choices: [
        { label: this.$t('userResearch.yesYouCanContactMe'), value: 'optIn' },
        { label: this.$t('userResearch.noDoNotContactMe'), value: 'optOut' },
      ],
      hasTriedToContinue: false,
      privacyPolicyUrl: this.$store.$env.PRIVACY_POLICY_URL,
      selectedValue: undefined,
      showNoSelectionError: false,
    };
  },
  computed: {
    showError() {
      return this.hasTriedToContinue && this.showNoSelectionError;
    },
  },
  methods: {
    async onContinue() {
      this.hasTriedToContinue = true;
      this.showNoSelectionError = isUndefined(this.selectedValue);

      if (!this.showError) {
        try {
          await this.$store.app.$http.postV1ApiUsersMeInfoUserresearch({
            userResearchRequest: { preference: this.selectedValue },
            ignoreError: true,
          });
        } catch {
          // do nothing
        } finally {
          this.$router.push({ path: NOTIFICATIONS_PATH, query: this.$route.query });
        }
      }
    },
    onSelection(value) {
      this.selectedValue = value;
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
