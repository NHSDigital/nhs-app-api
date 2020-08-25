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
               target="_blank"
               :text="$t('myAccount.accountSettings.nhsLogin')"
               :aria-label="$t('myAccount.accountSettings.nhsLogin')"
               :click-func="goToNHSSettings"/>
    <menu-item v-if="showNotifications"
               id="btn_notificationOptions"
               :header-tag="headerTag"
               :text="$t('myAccount.accountSettings.notificationOptions')"
               :aria-label="$t('myAccount.accountSettings.notificationOptions')"
               :click-func="showNotificationsClicked"/>
  </div>
</template>

<script>
import { ACCOUNT_NOTIFICATIONS_PATH, LOGIN_SETTINGS_PATH } from '@/router/paths';
import canVersionHandleBiometricsWeb from '@/lib/biometrics/canVersionHandleBiometricsWeb';
import NativeCallbacks from '@/services/native-app';
import MenuItem from '@/components/MenuItem';
import { redirectTo } from '@/lib/utils';
import { appLoginHelpUrl } from '@/router/externalLinks';

export default {
  name: 'Settings',
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
    };
  },
  computed: {
    getBiometricLinkText() {
      if (!canVersionHandleBiometricsWeb(this)) {
        return 'myAccount.accountSettings.passwordOptions';
      }

      if (this.$store.getters['loginSettings/getDeviceBiometricNameString'] !== undefined) {
        return this.$store.getters['loginSettings/getDeviceBiometricNameString'];
      }

      return 'loginSettings.biometrics.noBiometricType.settingsLinkText';
    },
  },
  methods: {
    goToLoginOptions() {
      if (canVersionHandleBiometricsWeb(this)) {
        redirectTo(this, LOGIN_SETTINGS_PATH);
      } else {
        this.configureWebContext(appLoginHelpUrl);
        NativeCallbacks.goToLoginOptions();
      }
    },
    showNotificationsClicked() {
      redirectTo(this, ACCOUNT_NOTIFICATIONS_PATH);
    },
    async goToNHSSettings() {
      const { token } = await this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: window.location.hostname,
          },
        });
      const settingsUrl = `${this.cidSettingsUrl}?asserted_login_identity=${token}`;

      if (this.isNativeApp) {
        if (NativeCallbacks.supportsNativeWebIntegration()) {
          NativeCallbacks.openWebIntegration(settingsUrl);
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
