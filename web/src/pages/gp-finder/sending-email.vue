<template>
  <div :class="[getHeaderState(), 'pull-content', $store.state.device.isNativeApp && $style.web]">
    <div>
      <h3 :class="[$style.h1]">
        {{ this.$t('th05.emailFeatureText') }}
      </h3>

      <form :class="$style.signup" @submit.prevent="signupFormSubmitted">
        <error-message v-if="choiceError" id="choice-error-label" role="alert"
                       aria-live="assertive">
          {{ this.$t('th05.choiceError') }}
        </error-message>
        <generic-radio-button :class="$style.choiceRadioButton"
                              :label="$t('th05.yesRadioButtonText')"
                              :selected-value="$store.state.throttling.waitingListChoice"
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
                                   :class="$style.throtlingLink"
                                   :text="$t('th05.privacyPolicyLinkText')"
                                   tag="a" target="_blank">
              {{ $t('th05.privacyPolicyLinkText') }}
            </analytics-tracked-tag>
          </p>
        </div>

        <generic-radio-button :class="[$style.choiceRadioButton, $style.last]"
                              :label="$t('th05.noRadioButtonText')"
                              :selected-value="$store.state.throttling.waitingListChoice"
                              value="no"
                              name="choice"
                              @select="radioButtonSelected"/>

        <analytics-tracked-tag :text="this.$t('th05.callToAction')" :tabindex="-1">
          <generic-button :button-classes="[$store.state.device.isNativeApp
                                              ?'button':'button-desktop',
                                            'green']" @click.prevent="signupFormSubmitted">
            {{ this.$t('th05.callToAction') }}
          </generic-button>
        </analytics-tracked-tag>
      </form>
    </div>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import NativeCallbacks from '@/services/native-app';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { GP_FINDER_PARTICIPATION, GP_FINDER_WAITING_LIST_JOINED } from '@/lib/routes';
import get from 'lodash/fp/get';

export default {
  components: {
    AnalyticsTrackedTag,
    ErrorMessage,
    GenericButton,
    GenericRadioButton,
    GenericTextInput,
  },
  data() {
    return {
      submitting: false,
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
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.showHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
    } else {
      window.scrollTo(0, 0);
    }
  },
  beforeCreate() {
    this.$store.dispatch('throttling/setWaitingListChoice', undefined);
  },
  methods: {
    getHeaderState() {
      return !this.$store.state.device.isNativeApp
        ? this.$style.webHeader : this.$style.nativeHeader;
    },
    backButtonClicked() {
      this.$store.dispatch('throttling/setWaitingListChoice', undefined);
      this.goToUrl(GP_FINDER_PARTICIPATION.path);
    },
    radioButtonSelected(value) {
      this.$store.dispatch('throttling/setWaitingListChoice', this.choice = value);
    },
    async signupFormSubmitted() {
      if (this.submitting) return;
      this.submitting = true;

      this.resetErrors();

      if (!this.choice) {
        this.submitting = false;
        this.choiceError = true;
        return;
      }

      if (this.choice === 'no') {
        this.submitting = false;
        this.goToUrl(GP_FINDER_WAITING_LIST_JOINED.path);
        return;
      }

      const emailIsValid = this.emailAddress && (this.emailAddress = this.emailAddress.trim());

      if (!emailIsValid) {
        this.submitting = false;
        this.notEnteredEmailError = true;
        return;
      }

      if (this.emailAddress.indexOf('@') === -1) {
        this.submitting = false;
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

      this.submitting = false;
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
  .webHeader {
    &.web {
      margin-top: -3.625em;
    }
  }

  .nativeHeader {
    padding: 0 0 3.125em 2.0px;
  }
  .throttlingContent {
    padding-top:0;
    padding-left:0;
  }

</style>
