<template>
  <div v-if="showTemplate">
    <menu-item v-if="showBiometrics"
               id="btn_passwordOptions"
               :header-tag="headerTag"
               :text="$t(getBiometricLinkText)"
               :aria-label="$t(getBiometricLinkText)"
               :click-func="goToLoginOptions"/>
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
    const biometricType = this.$t(this.$store.getters['loginSettings/getDeviceBiometricNameString']);
    return {
      biometricType,
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
  },
};
</script>
