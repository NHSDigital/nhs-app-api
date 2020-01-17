<template>
  <div v-if="showTemplate">
    <menu-item v-if="showBiometrics"
               id="btn_passwordOptions"
               :header-tag="headerTag"
               :text="$t('myAccount.accountSettings.passwordOptions')"
               :aria-label="$t('myAccount.accountSettings.passwordOptions')"
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
import { ACCOUNT_NOTIFICATIONS, findByName } from '@/lib/routes';
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
  methods: {
    goToLoginOptions() {
      this.configureWebContext(findByName('Login').helpUrl);
      NativeCallbacks.goToLoginOptions();
    },
    showNotificationsClicked() {
      this.$router.push(ACCOUNT_NOTIFICATIONS.path);
    },
  },
};
</script>
