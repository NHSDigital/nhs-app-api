<template>
  <terms-and-conditions-layout>
    <div v-if="showTemplate">
      <message-dialog v-if="showError" message-type="error" role="alert" tabindex="-1">
        <message-text data-purpose="error-heading">
          {{ $t('user_research.errorMessage.header') }}
        </message-text>
        <message-list data-purpose="reason-error">
          <li>
            {{ $t('user_research.errorMessage.text') }}
          </li>
        </message-list>
      </message-dialog>
      <p>{{ $t('user_research.contactYou') }}</p>
      <collapsible-details>
        <template slot="header">
          {{ $t('user_research.whatIsInvolved.header') }}
        </template>
        <p>{{ $t('user_research.whatIsInvolved.addYou') }}</p>
        <p>{{ $t('user_research.whatIsInvolved.signUp.label') }}</p>
        <ul>
          <li v-for="(benefit, index) in $t('user_research.whatIsInvolved.signUp.benefits')"
              :key="index">
            {{ benefit }}
          </li>
        </ul>
        <p>{{ $t('user_research.whatIsInvolved.signUp.isOptional') }}</p>
      </collapsible-details>
      <p>
        {{ $t('user_research.whatIsInvolved.restriction.prefix')
        }}<a class="inline"
             :href="privacyPolicyUrl"
             target="_blank"
             rel="noopener noreferrer">{{
               $t('user_research.whatIsInvolved.restriction.linkText') }}</a>{{
          $t('user_research.whatIsInvolved.restriction.suffix') }}
      </p>
      <radio-group :header="$t('user_research.question.label')"
                   header-size="m"
                   class="nhsuk-u-margin-bottom-4"
                   :show-error="showError"
                   :error-message="$t('user_research.errorMessage.text')"
                   :radios="choices"
                   @select="onSelection"/>
      <primary-button @click.stop.prevent="onContinue">
        {{ $t('user_research.continue') }}
      </primary-button>
    </div>
  </terms-and-conditions-layout>
</template>

<script>
import CollapsibleDetails from '@/components/CollapsibleDetails';
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
        { label: this.$t('user_research.question.yes'), value: 'optIn' },
        { label: this.$t('user_research.question.no'), value: 'optOut' },
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
