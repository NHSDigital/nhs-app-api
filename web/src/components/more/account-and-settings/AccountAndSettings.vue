<template>
  <div v-if="showTemplate">
    <menu-item v-if="showBiometrics"
               id="btn_passwordOptions"
               :header-tag="headerTag"
               :text="$t(getBiometricLinkText)"
               :aria-label="$t(getBiometricLinkText)"
               :click-func="goToLoginOptions"/>
    <menu-item id="btn_nhsLogin"
               :header-tag="headerTag"
               :target="(isNativeApp ? '_parent' : '_blank')"
               :text="$t('accountAndSettings.manageNhsLoginAccount')"
               :aria-label="$t('accountAndSettings.manageNhsLoginAccount')"
               :click-func="goToNHSSettings"/>
    <menu-item v-if="showNotifications"
               id="btn_notificationOptions"
               :header-tag="headerTag"
               :text="$t('accountAndSettings.manageNotifications')"
               :aria-label="$t('accountAndSettings.manageNotifications')"
               :click-func="showNotificationsClicked"/>
  </div>
</template>

<script>
import {
  MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
  MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH,
  MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH,
} from '@/router/paths';
import MenuItem from '@/components/MenuItem';
import NativeApp from '@/services/native-app';
import { redirectTo } from '@/lib/utils';
import biometricTypes from '@/lib/biometrics/biometricTypes';

export default {
  name: 'AccountAndSettings',
  components: {
    MenuItem,
  },
  props: {
    headerTag: {
      type: String,
      default: 'h2',
    },
    showBiometrics: {
      type: Boolean,
      required: true,
    },
    showNotifications: {
      type: Boolean,
      required: true,
    },
  },
  data() {
    return {
      cidSettingsUrl: this.$store.$env.CID_SETTINGS_URL,
      isNativeApp: this.$store.state.device.isNativeApp,
      settingsUrl: undefined,
    };
  },
  computed: {
    getBiometricLinkText() {
      if (this.$store.getters['loginSettings/biometricSupported']) {
        const biometricType = this.$store.getters['loginSettings/biometricType'];
        return `loginSettings.biometrics.biometricType.${biometricType}`;
      }

      return 'loginSettings.biometrics.noBiometricType.settingsLinkText';
    },
  },
  methods: {
    goToLoginOptions() {
      const biometricSupported = this.$store.getters['loginSettings/biometricSupported'];
      const biometricType = this.$store.getters['loginSettings/biometricType'];

      let redirectUrl = MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH;

      if (biometricSupported) {
        if (biometricType === biometricTypes.Fingerprint) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH;
        } else if (biometricType === biometricTypes.FaceID) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH;
        } else if (biometricType === biometricTypes.TouchID) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH;
        }
      }

      redirectTo(this, redirectUrl);
    },
    showNotificationsClicked() {
      redirectTo(this, MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH);
    },
    async goToNHSSettings() {
      const { token } = await this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: window.location.hostname,
          },
        });
      const settingsUrl = `${this.cidSettingsUrl}?asserted_login_identity=${token}`;
      this.configureWebContext('https://www.nhs.uk/nhs-app/nhs-app-help-and-support/nhs-app-account-and-settings/managing-your-nhs-app-account/');
      if (this.isNativeApp) {
        if (NativeApp.supportsNativeWebIntegration()) {
          NativeApp.openWebIntegration(settingsUrl);
        } else {
          window.location = settingsUrl;
        }
      } else {
        window.open(settingsUrl, '_blank', 'noopener,noreferrer');
      }
    },
  },
};
</script>
