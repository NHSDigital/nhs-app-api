<template>
  <div v-if="showTemplate">
    <div v-if="hasTriedToContinue && !areTermsAccepted" id="error_msg">
      <message-dialog :class="$style.customErrorBox"
                      :focusable="true"
                      message-type="error"
                      role="alert">
        <message-text>{{ $t('termsAndConditions.errors.youCannotContinue.header') }}</message-text>
        <message-list>
          <li>{{ $t('termsAndConditions.errors.youCannotContinue.text') }}</li>
        </message-list>
      </message-dialog>
    </div>
    <div id="text_body" :class="$style.info">
      <p> {{ $t('termsAndConditions.updated.weveMadeSomeChanges') }}
        <span>
          <!-- opening and closing tag must be on one line to
          avoid the inline-block white space issue
          -->
          <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.links.termsOfUse') }}</a>
        </span>,
        <span>
          <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.links.privacyPolicy') }}</a>
        </span>
        {{ $t('generic.and') }}
        <span>
          <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.links.cookiesPolicy') }}</a>
        </span>.
      </p>
      <p> {{ $t('termsAndConditions.updated.ifYouDoNotAgree') }} </p>
    </div>
    <div :class="getErrorState()">
      <error-message v-if="hasTriedToContinue && !areTermsAccepted"
                     id="error_txt"
                     role="alert"
                     :class="$style.validationText">
        {{ $t('termsAndConditions.updated.acceptConditionsCheckBox.youCannotUseWithoutAgreeing') }}
      </error-message>
      <fieldset class="nhsuk-fieldset">
        <legend>
          <generic-checkbox :value="termsAcceptedValue"
                            checkbox-id="termsAndConditions-agree_checkbox"
                            @input="termsSelectionChanged">
            <span :class="$style.termsAndConditionsCaption">
              {{ $t('termsAndConditions.updated.acceptConditionsCheckBox.understandAndAgree') }}
              <span>
                <!-- opening and closing tag must be on one line
                to avoid the inline-block white space issue -->
                <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer"
                   @click="stopProp($event)">{{ $t('termsAndConditions.links.termsOfUse') }}</a>
              </span>
              {{ $t('generic.and') }}
              <span>
                <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer"
                   @click="stopProp($event)">{{ $t('termsAndConditions.links.privacyPolicy') }}</a>
              </span>.
              {{ $t('termsAndConditions.updated.acceptConditionsCheckBox.acceptCookies') }}
              <span>
                <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer"
                   @click="stopProp($event)">{{ $t('termsAndConditions.links.cookiesPolicy') }}</a>
              </span>.
            </span>
          </generic-checkbox>
        </legend>
      </fieldset>
    </div>
    <generic-button id="btn_accept" :button-classes="['nhsuk-button']"
                    @click="onConfirmButtonClicked">
      {{ $t('termsAndConditions.updated.btnAccept') }}
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
import RedirectMixin from '@/components/RedirectMixin';
import {
  TERMS_AND_CONDITIONS_URL,
  PRIVACY_POLICY_URL,
  COOKIES_POLICY_URL,
} from '@/router/externalLinks';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { NOTIFICATIONS_PATH } from '@/router/paths';

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
  mixins: [RedirectMixin],
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
          this.$router.push({ path: NOTIFICATIONS_PATH, query: this.$route.query });
        }
      } else {
        EventBus.$emit(FOCUS_ERROR_ELEMENT);
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
  @import "../style/termsandconditions";
</style>
