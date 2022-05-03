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
               :text="$t('accountAndSettings.manageNhsAccount')"
               :href="settingsUrl"
               :aria-label="$t('accountAndSettings.manageNhsAccount')"
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
  MANAGING_YOUR_NHS_APP_ACCOUNT_HELP_PATH,
} from '@/router/externalLinks';
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
  async created() {
    const { token } = await this.$store.app.$http
      .postV1PatientAssertedLoginIdentity({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: window.location.hostname,
        },
      });
    this.settingsUrl = `${this.cidSettingsUrl}?asserted_login_identity=${token}`;
  },
  methods: {
    goToLoginOptions() {
      const biometricSupported = this.$store.getters['loginSettings/biometricSupported'];
      const biometricType = this.$store.getters['loginSettings/biometricType'];

      let redirectUrl = MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH;

      if (biometricSupported) {
        if (biometricType === biometricTypes.Fingerprint) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH;
        } else if (biometricType === biometricTypes.FingerprintFaceOrIris) {
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
      const helpUrl = `${this.$store.$env.BASE_NHS_APP_HELP_URL}${MANAGING_YOUR_NHS_APP_ACCOUNT_HELP_PATH}`;

      this.configureWebContext(helpUrl);
      if (this.isNativeApp) {
        if (NativeApp.supportsNativeWebIntegration()) {
          NativeApp.openWebIntegration(this.settingsUrl, [], helpUrl);
        } else {
          window.location = this.settingsUrl;
        }
      } else {
        window.open(this.settingsUrl, '_blank', 'noopener,noreferrer');
      }
    },
  },
};
</script>
