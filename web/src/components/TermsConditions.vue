<template>
  <div v-if="showTemplate" :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <div v-if="hasTriedToContinue && !areTermsAccepted" id="error_msg">
      <message-dialog :class="$style.customErrorBox" message-type="error"
                      role="alert">
        <p :class="$style.customErrorText"> {{ $t('termsAndConditions.errorMsgHeader') }} </p>
        <ul>
          <li> {{ $t('termsAndConditions.errorMsgText') }}</li>
        </ul>
      </message-dialog>
    </div>
    <div id="text_body" :class="$style.info">
      <p> {{ $t('termsAndConditions.body1') }}
        <span>
          <!-- inline links achieved through span to ensure font boosting is possible -->
          <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.link1') }}</a>
        </span>,
        <span>
          <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.link2') }}</a>
        </span>
        and
        <span>
          <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.link3') }}</a>
        </span>.
        {{ $t('termsAndConditions.body2') }} </p>
      <p> {{ $t('termsAndConditions.body3') }} </p>
      <p><strong> {{ $t('termsAndConditions.listTitle') }} </strong></p>
      <ul>
        <li v-for="listItem of $t('termsAndConditions.listItems')" :key="listItem">
          {{ listItem }}
        </li>
      </ul>
      <h2>{{ $t('termsAndConditions.cookiesTitle') }}</h2>
      <p>
        {{ $t('termsAndConditions.cookiesText1') }}
        <span>
          <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer">
            {{ $t('termsAndConditions.link4') }}</a>
        </span>
        {{ $t('termsAndConditions.cookiesText2') }}
      </p>
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
          {{ $t('termsAndConditions.checkBoxText1') }}

          <span>
            <!-- opening and closing tag must be on one line to
            avoid the inline-block white space issue - inline block
            prevents font boosting - accessibility issue
            -->
            <a :href="termsAndConditionsURL" target="_blank" rel="noopener noreferrer"
               @click="stopProp($event)">{{ $t('termsAndConditions.link1') }}</a>
          </span>
          and
          <span>
            <a :href="privacyPolicyURL" target="_blank" rel="noopener noreferrer"
               @click="stopProp($event)">{{ $t('termsAndConditions.link2') }}</a>.
          </span>
          {{ $t('termsAndConditions.checkBoxText2') }}
          <span>
            <a :href="cookiesPolicyURL" target="_blank" rel="noopener noreferrer"
               @click="stopProp($event)">{{ $t('termsAndConditions.link3') }}</a>.
          </span>
        </span>
      </generic-checkbox>
    </div>
    <generic-checkbox :value="analyticsAcceptedValue"
                      checkbox-id="analyticsCookie-agree_analyticsCookieCheckbox"
                      @input="analyticsSelectionChanged">
      {{ $t('termsAndConditions.analyticsCookieCheckBoxText') }}
    </generic-checkbox>
    <generic-button id="btn_accept" :button-classes="['nhsuk-button']"
                    @click="onConfirmButtonClicked">
      {{ $t('termsAndConditions.btnAccept') }}
    </generic-button>
  </div>
</template>

<script>
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import TermsConditionsMixin from '@/components/TermsConditionsMixin';
import { USER_RESEARCH } from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';

export default {
  name: 'TermsConditions',
  components: {
    GenericButton,
    ErrorMessage,
    MessageDialog,
    GenericCheckbox,
  },
  mixins: [TermsConditionsMixin],
  data() {
    return {
      termsAndConditionsURL: this.$store.app.$env.TERMS_AND_CONDITIONS_URL,
      privacyPolicyURL: this.$store.app.$env.PRIVACY_POLICY_URL,
      cookiesPolicyURL: this.$store.app.$env.COOKIES_POLICY_URL,
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

      if (isFalsy(this.$store.app.$env.USER_RESEARCH_ENABLED)) {
        this.conditionalRedirect();
      } else {
        redirectTo(this, USER_RESEARCH.path, this.$route.query);
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
    analyticsSelectionChanged() {
      this.isAnalyticsCookieAccepted = !this.isAnalyticsCookieAccepted;
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
