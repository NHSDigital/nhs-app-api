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
import { ACCOUNT_NOTIFICATIONS, findByName, LOGIN_SETTINGS } from '@/lib/routes';
import NativeCallbacks from '@/services/native-app';
import MenuItem from '@/components/MenuItem';

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
      webBiometricsEnabled: this.$store.app.$env.WEB_BIOMETRICS_ENABLED,
    };
  },
  computed: {
    getBiometricLinkText() {
      const isNativeVersionAfter = this.$store.getters['appVersion/isNativeVersionAfter'];
      if (!this.webBiometricsEnabled || !isNativeVersionAfter('1.34.0')) {
        return 'myAccount.accountSettings.passwordOptions';
      }

      if (this.$store.getters['loginSettings/getDeviceBiometricNameString'] !== undefined) {
        return this.$store.getters['loginSettings/getDeviceBiometricNameString'];
      }

      return 'loginSettings.biometrics.noBiometricType.settingsLinkText';
    },
    canVersionHandleBiometricsWeb() {
      const isNativeVersionAfter = this.$store.getters['appVersion/isNativeVersionAfter'];
      return !this.$store.state.device.isNativeApp || isNativeVersionAfter('1.34.0');
    },
  },
  methods: {
    goToLoginOptions() {
      if (this.canVersionHandleBiometricsWeb &&
        (this.webBiometricsEnabled || this.$store.state.device.source === 'ios')) {
        this.$router.push(LOGIN_SETTINGS.path);
      } else {
        this.configureWebContext(findByName('Login').helpUrl);
        NativeCallbacks.goToLoginOptions();
      }
    },
    showNotificationsClicked() {
      this.$router.push(ACCOUNT_NOTIFICATIONS.path);
    },
  },
};
</script>
