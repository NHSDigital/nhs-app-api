<template>
  <div v-if="showTemplate" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
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
      <p>{{ $t('termsAndConditions.initial.youMustAgreeTo') }}
        <span>
          <!-- inline links achieved through span to ensure font boosting is possible -->
          <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.links.termsOfUse') }}</a>
        </span>,
        <span>
          <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.links.privacyPolicy') }}</a>
        </span>
        {{ $t('generic.and' ) }}
        <span>
          <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.links.cookiesPolicy') }}</a>
        </span>.
        {{ $t('termsAndConditions.initial.youShouldReadCarefully') }} </p>
      <p>{{ $t('termsAndConditions.initial.ifYouDoNotAgree') }}</p>
      <p><strong> {{ $t('termsAndConditions.initial.keyPoints.title') }} </strong></p>
      <ul>
        <li v-for="listItem of $t('termsAndConditions.initial.keyPoints.items')" :key="listItem">
          {{ listItem }}
        </li>
      </ul>
      <h2>{{ $t('termsAndConditions.initial.manageCookies.title') }}</h2>
      <p>
        {{ $t('termsAndConditions.initial.manageCookies.prefix') }}
        <span>
          <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.initial.manageCookies.link') }}</a>
        </span>
        {{ $t('termsAndConditions.initial.manageCookies.suffix') }}
      </p>
    </div>
    <div :class="getErrorState()">
      <error-message v-if="hasTriedToContinue && !areTermsAccepted"
                     id="error_txt"
                     role="alert"
                     :class="$style.validationText">
        {{ $t('termsAndConditions.initial.acceptConditionsCheckBox.youCannotUseWithoutAgreeing') }}
      </error-message>
      <fieldset class="nhsuk-fieldset">
        <legend>
          <generic-checkbox :value="termsAcceptedValue"
                            checkbox-id="termsAndConditions-agree_checkbox"
                            @onCheckedChanged="termsSelectionChanged">
            <span :class="$style.termsAndConditionsCaption">
              {{ $t('termsAndConditions.initial.acceptConditionsCheckBox.understandAndAccept') }}

              <span>
                <!-- opening and closing tag must be on one line to
                avoid the inline-block white space issue - inline block
                prevents font boosting - accessibility issue
                -->
                <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer"
                   @click="stopProp($event)">{{ $t('termsAndConditions.links.termsOfUse') }}</a>
              </span>
              {{ $t('generic.and') }}
              <span>
                <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer"
                   @click="stopProp($event)">{{ $t('termsAndConditions.links.privacyPolicy') }}</a>.
              </span>
              {{ $t('termsAndConditions.initial.acceptConditionsCheckBox.acceptCookies') }}
              <span>
                <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer"
                   @click="stopProp($event)">{{ $t('termsAndConditions.links.cookiesPolicy') }}</a>.
              </span>
            </span>
          </generic-checkbox>
        </legend>
      </fieldset>
    </div>
    <generic-checkbox :value="analyticsAcceptedValue"
                      checkbox-id="analyticsCookie-agree_analyticsCookieCheckbox"
                      @onCheckedChanged="analyticsSelectionChanged">
      {{ $t('termsAndConditions.initial.analyticsCookieCheckBox.text') }}
    </generic-checkbox>
    <generic-button id="btn_accept" :button-classes="['nhsuk-button']"
                    @click="onConfirmButtonClicked">
      {{ $t('termsAndConditions.initial.btnAccept') }}
    </generic-button>
  </div>
</template>

<script>
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import TermsConditionsMixin from '@/components/TermsConditionsMixin';
import { USER_RESEARCH_PATH } from '@/router/paths';
import { isFalsy } from '@/lib/utils';
import {
  TERMS_AND_CONDITIONS_URL,
  PRIVACY_POLICY_URL,
  COOKIES_POLICY_URL,
} from '@/router/externalLinks';

export default {
  name: 'TermsConditions',
  components: {
    GenericButton,
    ErrorMessage,
    MessageDialog,
    MessageList,
    MessageText,
    GenericCheckbox,
  },
  mixins: [TermsConditionsMixin],
  data() {
    return {
      termsAndConditionsURL: TERMS_AND_CONDITIONS_URL,
      privacyPolicyURL: PRIVACY_POLICY_URL,
      cookiesPolicyURL: COOKIES_POLICY_URL,
      areTermsAccepted: false,
      isAnalyticsCookieAccepted: false,
      termsAcceptedValue: 'terms',
      analyticsAcceptedValue: 'analytics',
      hasTriedToContinue: false,
    };
  },
  methods: {
    stopProp(event) {
      event.stopPropagation();
    },
    async onConfirmButtonClicked() {
      this.hasTriedToContinue = true;

      if (!this.areTermsAccepted) {
        window.scrollTo(0, 0);
        return;
      }

      const consentRequest = {
        ConsentGiven: true,
        AnalyticsCookieAccepted: this.isAnalyticsCookieAccepted,
      };

      await this.$store.dispatch('termsAndConditions/acceptTerms', { consentRequest });

      if (!this.$store.state.termsAndConditions.areAccepted) {
        return;
      }

      if (isFalsy(this.$store.$env.USER_RESEARCH_ENABLED)) {
        this.conditionalRedirect();
      } else {
        this.$router.push({ path: USER_RESEARCH_PATH });
      }
    },
    getErrorState() {
      if (this.hasTriedToContinue && !this.areTermsAccepted) {
        return this.$style.validationBorderLeft;
      }
      return null;
    },
    termsSelectionChanged(isChecked) {
      this.areTermsAccepted = isChecked;
    },
    analyticsSelectionChanged(isChecked) {
      this.isAnalyticsCookieAccepted = isChecked;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/termsConditions";

  .acceptButton {
    &.desktopWeb {
      margin: 1em 0;
    }
  }
</style>
