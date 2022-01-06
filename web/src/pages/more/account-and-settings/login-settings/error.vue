<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="error === cannotFindBiometricsErrorCode" class="nhsuk-u-padding-top-4">
        <p data-sid="cannotFindBiometrics">
          {{ $t(cannotFindErrorText) }}
        </p>
        <p v-if="isAndroid">
          {{ $t('loginSettings.biometrics.errors.cannotFindBiometricType.weCannotSupport') }}
        </p>
        <p>
          <a :href="helpLink" target="_blank"
             rel="noopener noreferrer">
            {{ $t('loginSettings.biometrics.errors.cannotFindBiometricType.getHelp') }}
          </a>
        </p>
        <p>
          {{ $t('loginSettings.biometrics.errors.cannotFindBiometricType.ifYouCantUseBiometrics') }}
        </p>
      </div>
      <div v-if="error === cannotChangeBiometricsErrorCode" class="nhsuk-u-padding-top-4">
        <p data-sid="cannotChangeBiometricsParagraphOne">
          {{ $t('loginSettings.biometrics.errors.cannotChangeBiometricSettings.paragraph1') }}
        </p>
        <p data-sid="cannotChangeBiometricsParagraphTwo">
          {{ $t('loginSettings.biometrics.errors.cannotChangeBiometricSettings.paragraph2') }}
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import { MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_PATH } from '@/router/paths';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import { redirectTo } from '@/lib/utils';
import { NHS_APP_LOGGING_IN_HELP } from '@/router/externalLinks';

export default {
  name: 'LoginSettingsErrorPage',
  data() {
    const error = this.$store.getters['loginSettings/biometricError'];
    const biometricType = this.$store.getters['loginSettings/biometricType'];
    return {
      error,
      hasError: error !== undefined,
      cannotFindErrorText: `loginSettings.biometrics.errors.cannotFindBiometricType.errorText.${biometricType}`,
      cannotFindBiometricsErrorCode: biometricErrorCodes.CannotFindBiometrics,
      cannotChangeBiometricsErrorCode: biometricErrorCodes.CannotChangeBiometrics,
      helpLink: NHS_APP_LOGGING_IN_HELP,
    };
  },
  computed: {
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
  },
  created() {
    if (this.$store.getters['loginSettings/biometricError'] === undefined) {
      redirectTo(this, MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_PATH);
    }
  },
};
</script>
