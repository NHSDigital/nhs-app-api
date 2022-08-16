<template>
  <no-return-flow-layout>
    <div v-if="showTemplate" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="error === cannotChangeBiometricsErrorCode" class="nhsuk-u-padding-top-4">
          <p data-sid="cannotChangeBiometricsParagraphOne">
            {{ $t(technicalProblemText) }}
          </p>
          <p data-sid="cannotChangeBiometricsParagraphTwo">
            {{ $t('biometricsRegistration.errors.cannotTurnOnBiometrics.tryAgainLater') }}
          </p>
          <p data-sid="cannotChangeBiometricsParagraphThree">
            {{ $t(ifYouStillCannotTurnOn) }}
            <a :href="helpLink" target="_blank"
               rel="noopener noreferrer" :class="$style['inline-hyperlink']">
              {{ $t('biometricsRegistration.errors.cannotTurnOnBiometrics.getHelpWithLoggingIn') }}</a>.
          </p>
          <p data-sid="cannotChangeBiometricsParagraphFour">
            {{ $t(loginUsingOtherMeans) }}
          </p>
        </div>
        <div v-if="error === cannotFindBiometricsErrorCode" class="nhsuk-u-padding-top-4">
          <p data-sid="cannotFindBiometricsParagraphOne">
            {{ $t('biometricsRegistration.errors.cannotFindBiometrics.checkYourDevice') }}
          </p>
          <ul data-sid="cannotFindBiometricsList">
            <li>{{ $t(turnedOnText) }}</li>
            <li>{{ $t(addBiometricsText) }}</li>
          </ul>
          <p data-sid="cannotFindBiometricsParagraphTwo">
            {{ $t(ifYouCannotUseText) }}
          </p>
        </div>
        <generic-button id="continueButton"
                        :button-classes="['nhsuk-button']"
                        @click="continueButtonClicked">
          {{ $t('generic.continue') }}
        </generic-button>
      </div>
    </div>
  </no-return-flow-layout>
</template>

<script>
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import { NHS_APP_LOGGING_IN_HELP } from '@/router/externalLinks';
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import GenericButton from '@/components/widgets/GenericButton';
import NativeApp from '@/services/native-app';

export default {
  name: 'BiometricsError',
  components: {
    NoReturnFlowLayout,
    GenericButton,
  },
  data() {
    const error = this.$store.getters['loginSettings/biometricError'];
    const biometricType = this.$store.getters['loginSettings/biometricType'];
    return {
      error,
      hasError: error !== undefined,
      cannotChangeBiometricsErrorCode: biometricErrorCodes.CannotChangeBiometrics,
      technicalProblemText: `biometricsRegistration.errors.cannotTurnOnBiometrics.technicalProblem.${biometricType}`,
      ifYouStillCannotTurnOn: `biometricsRegistration.errors.cannotTurnOnBiometrics.ifYouStillCannotTurnOn.${biometricType}`,
      loginUsingOtherMeans: `biometricsRegistration.errors.cannotTurnOnBiometrics.loginUsingOtherMeans.${biometricType}`,
      helpLink: NHS_APP_LOGGING_IN_HELP,
      cannotFindBiometricsErrorCode: biometricErrorCodes.CannotFindBiometrics,
      turnedOnText: `biometricsRegistration.errors.cannotFindBiometrics.turnOnBiometrics.${biometricType}`,
      addBiometricsText: `biometricsRegistration.errors.cannotFindBiometrics.addedBiometrics.${biometricType}`,
      ifYouCannotUseText: `biometricsRegistration.errors.cannotFindBiometrics.ifYouCannotUse.${biometricType}`,
    };
  },
  created() {
    if (this.$store.getters['loginSettings/biometricError'] === undefined) {
      NativeApp.goToLoggedInHomeScreen();
    }
  },
  methods: {
    continueButtonClicked() {
      NativeApp.goToLoggedInHomeScreen();
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "@/style/custom/a-inline";
</style>
