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
      <p :class="$style.contactYouText">{{ this.$t('th05.contactYouText') }}</p>
      <h4>{{ this.$t('th05.emailText') }}</h4>
      <form id="signup" :action="callBrotherMailer"
            name="signup" autocomplete="off">
        <error-message v-if="showError" id="error-label">
          {{ errorText }}
        </error-message>
        <generic-text-input id="emailInput"
                            ref="email"
                            :class="$style.searchTextInput"
                            :type="'text'"
                            :a-labelled-by="'search-label'"
                            name="email"
                            input-name="email"
                            maxlength="256"
        />
        <input id="odsCode" :value="odsCode" type="hidden" name="odsCode" >
        <generic-button :class="[$style.button, $style.green]" :type="'submit'">
          {{ this.$t('th05.continueButton') }}
        </generic-button>
      </form>
      <generic-button :class="[$style.button]" @click="onReturnHomeClicked">
        {{ this.$t('th05.homeButton') }}
      </generic-button>
    </div>
  </div>
</template>

<script>
import BackIcon from '@/components/icons/BackIcon';
import HeaderSlim from '@/components/HeaderSlim';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import { BROTHERMAILER, LOGIN, GP_FINDER, GP_FINDER_PARTICIPATION } from '@/lib/routes';

export default {
  layout: 'throttling',
  components: {
    HeaderSlim,
    BackIcon,
    GenericTextInput,
    GenericButton,
    ErrorMessage,
  },
  data() {
    return {
      returnPath: undefined,
      hostPath: undefined,
      gpLookUpBrotherMailerURL: this.$store.app.$env.GP_LOOKUP_BROTHER_MAILER_URL,
      odsCode: undefined,
      practiceName: undefined,
      practiceAddress: undefined,
      connectionError: false,
      invalidEmailError: false,
      submissionError: false,
      headerText: this.$t('th05.header'),
      backLink: GP_FINDER_PARTICIPATION.path,
    };
  },
  asyncData(context) {
    const cookie = context.store.$cookies.get('BetaCookie');
    if (!cookie || !cookie.ODSCode) {
      context.redirect(`${GP_FINDER.path}?reset=true`);
    }

    if (context.query.error === 'connectionError') {
      return { connectionError: true };
    }

    if (context.query.error === 'invalidEmailError') {
      return { invalidEmailError: true };
    }

    if (context.query.error === 'submissionError') {
      return { submissionError: true };
    }

    return {
      odsCode: cookie.ODSCode,
      practiceName: cookie.PracticeName,
      practiceAddress: cookie.PracticeAddress,
    };
  },
  computed: {
    showError() {
      return this.submissionError || this.connectionError || this.invalidEmailError;
    },
    callBrotherMailer() {
      return BROTHERMAILER.path;
    },
    errorText() {
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
      this.$router.push(`${LOGIN.path}`);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/elements';
@import '../../style/buttons';
@import '../../style/throttling/throttling';
@import '../../style/throttling/gpfindersendemail';
@import '../../style/headerslim';
</style>
