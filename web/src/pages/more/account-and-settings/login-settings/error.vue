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
      <div v-if="cannotUseBiometrics" class="nhsuk-u-padding-top-4">
        <p>
          {{ $t(`${CannotUseBiometricsText}.youNeedTo`) }}
          <a id="app-turn-on-biometric-type" href="#" :class="$style['inline-hyperlink']" @click.prevent.stop="openAppSettings">
            {{ $t(`${CannotUseBiometricsText}.turnOnBiometricsInYourDeviceSettingsLink`) }}</a>
          {{ $t(`${CannotUseBiometricsText}.beforeYouCanTurnOnBiometricsInNHSApp`) }}
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
import NativeApp from '@/services/native-app';

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
      cannotUseBiometricErrorCode: biometricErrorCodes.CannotUseBiometrics,
      CannotUseBiometricsText: `loginSettings.biometrics.errors.cannotUseBiometricSettings.${biometricType}`,
      helpLink: NHS_APP_LOGGING_IN_HELP,
    };
  },
  computed: {
    isAndroid() {
      return this.$store.state.device.source === 'android';
    },
    cannotUseBiometrics() {
      return !this.isAndroid && this.error === this.cannotUseBiometricErrorCode;
    },
  },
  created() {
    if (this.$store.getters['loginSettings/biometricError'] === undefined) {
      redirectTo(this, MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_PATH);
    }
  },
  methods: {
    openAppSettings() {
      NativeApp.openAppSettings();
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/a-inline";
</style>

