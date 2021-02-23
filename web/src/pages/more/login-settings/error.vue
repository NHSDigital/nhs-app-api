<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="error === cannotFindBiometricsErrorCode" class="nhsuk-u-padding-top-4">
        <p data-sid="cannotFindBiometrics">
          {{ $t(cannotFindErrorText) }}
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
import { MORE_LOGIN_SETTINGS_PATH } from '@/router/paths';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'LoginSettingsErrorPage',
  data() {
    return {
      error: this.$store.getters['loginSettings/retrieveError'],
      hasError: this.$store.getters['loginSettings/retrieveError'] !== undefined,
      cannotFindErrorText: this.$store.getters['loginSettings/retrieveCannotFindErrorText'],
      cannotFindBiometricsErrorCode: biometricErrorCodes.CannotFindBiometrics,
      cannotChangeBiometricsErrorCode: biometricErrorCodes.CannotChangeBiometrics,
    };
  },
  created() {
    if (this.$store.state.loginSettings.errorCode === undefined) {
      redirectTo(this, MORE_LOGIN_SETTINGS_PATH);
    }
  },
};
</script>
