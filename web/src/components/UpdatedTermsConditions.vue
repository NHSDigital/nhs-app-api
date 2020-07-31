<template>
  <div v-if="showTemplate">
    <div v-if="hasTriedToContinue && !areTermsAccepted" id="error_msg">
      <message-dialog :class="$style.customErrorBox"
                      message-type="error"
                      role="alert">
        <message-text>
          {{ $t('updatedTermsAndConditions.errorMsgHeader') }}
        </message-text>
        <message-list>
          <li> {{ $t('updatedTermsAndConditions.errorMsgText') }} </li>
        </message-list>
      </message-dialog>
    </div>
    <div id="text_body" :class="$style.info">
      <p> {{ $t('updatedTermsAndConditions.body1') }}
        <span>
          <!-- opening and closing tag must be on one line to
          avoid the inline-block white space issue
          -->
          <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer">
            {{ $t('updatedTermsAndConditions.link1') }}</a>
        </span>,
        <span>
          <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('updatedTermsAndConditions.link2') }}</a>
        </span>
        and
        <span>
          <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('updatedTermsAndConditions.link3') }}</a>
        </span>.
      </p>
      <p> {{ $t('updatedTermsAndConditions.body2') }} </p>
    </div>
    <div :class="getErrorState()">
      <error-message v-if="hasTriedToContinue && !areTermsAccepted"
                     id="error_txt"
                     role="alert"
                     :class="$style.validationText">
        {{ $t('termsAndConditions.checkBoxError') }}
      </error-message>
      <generic-checkbox :value="termsAcceptedValue"
                        checkbox-id="termsAndConditions-agree_checkbox"
                        @input="termsSelectionChanged">
        <span :class="$style.termsAndConditionsCaption">
          {{ $t('updatedTermsAndConditions.checkBoxText1') }}
          <span>
            <!-- opening and closing tag must be on one line
            to avoid the inline-block white space issue -->
            <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer"
               @click="stopProp($event)">{{ $t('updatedTermsAndConditions.link1') }}</a>
          </span>
          and
          <span>
            <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer"
               @click="stopProp($event)">{{ $t('updatedTermsAndConditions.link2') }}</a>
          </span>.
          {{ $t('updatedTermsAndConditions.checkBoxText2') }}
          <span>
            <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer"
               @click="stopProp($event)">{{ $t('updatedTermsAndConditions.link3') }}</a>
          </span>.
        </span>
      </generic-checkbox>
    </div>
    <generic-button id="btn_accept" :button-classes="['nhsuk-button']"
                    @click="onConfirmButtonClicked">
      {{ $t('updatedTermsAndConditions.btnAccept') }}
    </generic-button>
  </div>
</template>

<script>
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import TermsConditionsMixin from '@/components/TermsConditionsMixin';
import {
  TERMS_AND_CONDITIONS_URL,
  PRIVACY_POLICY_URL,
  COOKIES_POLICY_URL,
} from '@/router/externalLinks';

export default {
  name: 'UpdatedTermsConditions',
  components: {
    ErrorMessage,
    GenericButton,
    GenericCheckbox,
    MessageDialog,
    MessageList,
    MessageText,
  },
  mixins: [TermsConditionsMixin],
  data() {
    return {
      termsAndConditionsURL: TERMS_AND_CONDITIONS_URL,
      privacyPolicyURL: PRIVACY_POLICY_URL,
      cookiesPolicyURL: COOKIES_POLICY_URL,
      areTermsAccepted: false,
      termsAcceptedValue: 'terms',
      hasTriedToContinue: false,
    };
  },
  methods: {
    stopProp(event) {
      event.stopPropagation();
    },
    async onConfirmButtonClicked() {
      this.hasTriedToContinue = true;
      if (this.areTermsAccepted) {
        const consentRequest = {
          ConsentGiven: true,
          UpdatingConsent: true,
        };

        await this.$store.dispatch('termsAndConditions/acceptTerms', { consentRequest });

        if (this.$store.state.termsAndConditions.areAccepted) {
          this.conditionalRedirect();
        }
      } else {
        window.scrollTo(0, 0);
      }
    },
    getErrorState() {
      if (this.hasTriedToContinue && !this.areTermsAccepted) {
        return this.$style.validationBorderLeft;
      }
      return null;
    },
    termsSelectionChanged() {
      this.areTermsAccepted = !this.areTermsAccepted;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/termsConditions";
</style>
