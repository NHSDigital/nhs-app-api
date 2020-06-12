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
import canVersionHandleBiometricsWeb from '@/lib/biometrics/canVersionHandleBiometricsWeb';
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
