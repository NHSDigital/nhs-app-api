<template>
  <div>
    <header :class="[$style.slim]">
      <h1 :class="[$style.h1]"> {{ headerText }} </h1>
      <form :action="backLink" method="get">
        <input :value="odsCode" type="hidden" name="odsCode">
        <input :value="practiceName" type="hidden" name="practiceName">
        <input :value="practiceAddress" type="hidden" name="practiceAddress">
        <input :value="this.$store.state.device.source" type="hidden" name="source">
        <button type="submit">
          <back-icon/>
        </button>
      </form>
    </header>
    <div v-if="showTemplate" id="mainDiv" :class="[$style.webHeader,
                                                   $style.throttlingContent, 'pull-content']">
      <h3>{{ this.$t('th05.emailFeatureText') }}</h3>
      <form id="signup" :class="$style.signup"
            :action="`${callBrotherMailer}?source=${this.$store.state.device.source}`"
            method="post" name="signup" autocomplete="off">
        <error-message v-if="choiceError" id="choice-error-label"
                       role="alert" aria-live="assertive">
          {{ this.$t('th05.choiceError') }}
        </error-message>
        <generic-radio-button :class="$style.choiceRadioButton"
                              :label="$t('th05.yesRadioButtonText')"
                              :model="choice"
                              value="yes"
                              name="choice"
                              @select="radioButtonSelected"/>
        <div :class="$style['radio-adjusted']">
          <h4 id="email-label">{{ this.$t('th05.emailText') }}</h4>
          <error-message v-if="showError" id="error-label" role="alert" aria-live="assertive">
            {{ errorText }}
          </error-message>
          <generic-text-input id="emailInput"
                              ref="email"
                              :type="'text'"
                              :a-labelled-by="emailInputLabelledBy"
                              name="email"
                              maxlength="255"
          />
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
        <generic-radio-button :class="$style.choiceRadioButton"
                              :label="$t('th05.noRadioButtonText')"
                              :model="choice"
                              value="no"
                              name="choice"
                              @select="radioButtonSelected"/>
        <input id="odsCode" :value="odsCode" type="hidden" name="odsCode">
        <generic-button :class="[$style.button, $style.green]" :type="'submit'">
          {{ this.$t('th05.continueButton') }}
        </generic-button>
      </form>
    </div>
  </div>
</template>

<script>
import BackIcon from '@/components/icons/BackIcon';
import HeaderSlim from '@/components/HeaderSlim';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { BROTHERMAILER_SIGNUP_NOJS, LOGIN, GP_FINDER, GP_FINDER_PARTICIPATION } from '@/lib/routes';

export default {
  layout: 'throttling',
  components: {
    HeaderSlim,
    BackIcon,
    GenericTextInput,
    GenericButton,
    GenericRadioButton,
    ErrorMessage,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      returnPath: undefined,
      hostPath: undefined,
      odsCode: undefined,
      practiceName: undefined,
      practiceAddress: undefined,
      connectionError: false,
      invalidEmailError: false,
      notEnteredEmailError: false,
      submissionError: false,
      choiceError: false,
      choice: undefined,
      headerText: this.$store.state.header.headerText,
      backLink: GP_FINDER_PARTICIPATION.path,
      privacyPolicyURL: this.$store.app.$env.PRIVACY_POLICY_URL,
    };
  },
  asyncData(context) {
    const cookie = context.store.$cookies.get('BetaCookie');
    if (!cookie || !cookie.ODSCode) {
      context.redirect(`${GP_FINDER.path}?reset=true`);
    }

    const data = {
      odsCode: cookie.ODSCode,
      practiceName: cookie.PracticeName,
      practiceAddress: cookie.PracticeAddress,
      connectionError: false,
      invalidEmailError: false,
      notEnteredEmailError: false,
      submissionError: false,
      choiceError: false,
      choice: undefined,
    };

    switch (context.query.error) {
      case 'choiceError':
        data.choiceError = true;
        break;
      case 'connectionError':
        data.connectionError = true;
        break;
      case 'invalidEmailError':
        data.invalidEmailError = true;
        data.choice = 'yes';
        break;
      case 'notEnteredEmailError':
        data.notEnteredEmailError = true;
        data.choice = 'yes';
        break;
      case 'submissionError':
        data.submissionError = true;
        break;
      default:
        break;
    }

    return data;
  },
  computed: {
    showError() {
      return this.submissionError || this.connectionError ||
             this.invalidEmailError || this.notEnteredEmailError;
    },
    emailInputLabelledBy() {
      return this.showError ? 'email-label error-label' : 'email-label';
    },
    callBrotherMailer() {
      return BROTHERMAILER_SIGNUP_NOJS.path;
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
  methods: {
    onReturnHomeClicked() {
      this.$router.push(LOGIN.path);
    },
    radioButtonSelected(value) {
      if (this.choice !== value) {
        this.choice = value;
      }
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
