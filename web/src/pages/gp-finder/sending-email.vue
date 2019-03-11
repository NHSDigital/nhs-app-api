<template>
  <div>

    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ getHeaderText }} </h1>
      <analytics-tracked-tag text="back">
        <button @click="backButtonClicked">
          <back-icon/>
        </button>
      </analytics-tracked-tag>
    </header>

    <div :class="[$style.webHeader, $style.throttlingContent, 'pull-content']">
      <h3>{{ this.$t('th05.emailFeatureText') }}</h3>

      <form :class="$style.signup" @submit.prevent="joinWaitingListFormSubmitted">
        <error-message v-if="choiceError" id="choice-error-label" role="alert"
                       aria-live="assertive">
          {{ this.$t('th05.choiceError') }}
        </error-message>
        <generic-radio-button :class="$style.choiceRadioButton"
                              :label="$t('th05.yesRadioButtonText')"
                              value="yes"
                              name="choice"
                              @select="radioButtonSelected"/>

        <div :class="$style['radio-adjusted']">
          <h4 id="email-label">{{ this.$t('th05.emailText') }}</h4>

          <error-message v-if="showError" id="error-label" role="alert" aria-live="assertive">
            {{ errorText }}
          </error-message>

          <generic-text-input id="emailInput"
                              v-model="emailAddress"
                              :type="'text'"
                              :a-labelled-by="emailInputLabelledBy"
                              name="email"
                              maxlength="255"/>

          <p :class="$style.privacyStatement"
             :aria-label="$t('th05.privacyStatement') + $t('th05.privacyPolicyLinkText')">
            {{ $t('th05.privacyStatement') }}
            <analytics-tracked-tag :href="privacyPolicyURL"
                                   :class="$style['paragraph-link']"
                                   :text="$t('th05.privacyPolicyLinkText')"
                                   tag="a" target="_blank">
              {{ $t('th05.privacyPolicyLinkText') }}
            </analytics-tracked-tag>
          </p>
        </div>

        <generic-radio-button :class="[$style.choiceRadioButton, $style.last]"
                              :label="$t('th05.noRadioButtonText')"
                              value="no"
                              name="choice"
                              @select="radioButtonSelected"/>

        <analytics-tracked-tag :text="this.$t('th05.callToAction')">
          <generic-button :button-classes="['green', 'button']" @click="callToActionClicked">
            {{ this.$t('th05.callToAction') }}
          </generic-button>
        </analytics-tracked-tag>
      </form>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import BackIcon from '@/components/icons/BackIcon';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { GP_FINDER, GP_FINDER_PARTICIPATION, GP_FINDER_WAITING_LIST_JOINED } from '@/lib/routes';
import get from 'lodash/fp/get';

export default {
  layout: 'throttling',
  components: {
    AnalyticsTrackedTag,
    BackIcon,
    ErrorMessage,
    GenericButton,
    GenericRadioButton,
    GenericTextInput,
  },
  data() {
    return {
      connectionError: false,
      invalidEmailError: false,
      notEnteredEmailError: false,
      submissionError: false,
      choiceError: false,
      emailAddress: undefined,
      choice: undefined,
      privacyPolicyURL: this.$store.app.$env.PRIVACY_POLICY_URL,
    };
  },
  computed: {
    showError() {
      return this.submissionError || this.connectionError ||
             this.invalidEmailError || this.notEnteredEmailError;
    },
    getHeaderText() {
      return this.$store.state.header.headerText;
    },
    emailInputLabelledBy() {
      return this.showError ? 'email-label error-label' : 'email-label';
    },
    errorText() {
      if (this.notEnteredEmailError) {
        return this.$t('th05.notEnteredEmailError');
      }

      if (this.invalidEmailError) {
        return this.$t('th05.invalidEmailError');
      }

      if (this.submissionError) {
        return this.$t('th05.submissionError');
      }

      if (this.connectionError) {
        return this.$t('th05.connectionError');
      }

      return undefined;
    },
  },
  mounted() {
    if (!get('selectedGpPractice.ODSCode')(this.$store.state.throttling)) {
      this.goToUrl(GP_FINDER.path);
    }
  },
  methods: {
    backButtonClicked() {
      this.$store.dispatch('throttling/setWaitingListChoice', undefined);
      this.goToUrl(GP_FINDER_PARTICIPATION.path);
    },
    radioButtonSelected(value) {
      this.$store.dispatch('throttling/setWaitingListChoice', this.choice = value);
    },
    async callToActionClicked() {
      if (this.continueClicked) return;
      this.continueClicked = true;

      this.resetErrors();

      if (!this.choice) {
        this.continueClicked = false;
        this.choiceError = true;
        return;
      }

      if (this.choice === 'no') {
        this.continueClicked = false;
        this.goToUrl(GP_FINDER_WAITING_LIST_JOINED.path);
        return;
      }

      const emailIsValid = this.emailAddress && (this.emailAddress = this.emailAddress.trim());

      if (!emailIsValid) {
        this.continueClicked = false;
        this.notEnteredEmailError = true;
        return;
      }

      if (this.emailAddress.indexOf('@') === -1) {
        this.continueClicked = false;
        this.invalidEmailError = true;
        return;
      }

      const brothermailerRequest = {
        odsCode: get('selectedGpPractice.ODSCode')(this.$store.state.throttling),
        emailAddress: this.emailAddress,
      };

      await this.$store.app.$http.postV1Brothermailer({ brothermailerRequest })
        .then(() => {
          this.goToUrl(GP_FINDER_WAITING_LIST_JOINED.path);
        })
        .catch((error) => {
          if (!error.response || !error.response.status ||
            error.response.status === 502 || error.response.status === 500) {
            this.submissionError = true;
            return;
          }
          this.invalidEmailError = true;
        });

      this.continueClicked = false;
    },
    joinWaitingListFormSubmitted() {
      this.callToActionClicked();
    },
    resetErrors() {
      this.choiceError = false;
      this.notEnteredEmailError = false;
      this.invalidEmailError = false;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/forms';
@import '../../style/elements';
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfindersendemail';
@import '../../style/headerslim';
</style>
