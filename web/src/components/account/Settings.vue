<template>
  <div v-if="showTemplate">
    <menu-item-list>
      <menu-item v-if="showBiometrics"
                 id="btn_passwordOptions"
                 :text="$t('myAccount.accountSettings.passwordOptions')"
                 :aria-label="$t('myAccount.accountSettings.passwordOptions')"
                 :click-func="goToLoginOptions"/>

      <menu-item v-if="showNotifications"
                 id="btn_notificationOptions"
                 :text="$t('myAccount.accountSettings.notificationOptions')"
                 :aria-label="$t('myAccount.accountSettings.notificationOptions')"
                 :click-func="showNotificationsClicked"/>
    </menu-item-list>
  </div>
</template>

<script>
import { ACCOUNT_NOTIFICATIONS, findByName } from '@/lib/routes';
import NativeCallbacks from '@/services/native-app';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';

export default {
  name: 'Settings',
  components: {
    MenuItem,
    MenuItemList,
  },
  props: {
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
      this.setHelpUrl(findByName('Login').helpUrl);
      NativeCallbacks.goToLoginOptions();
    },
    showNotificationsClicked() {
      this.$router.push(ACCOUNT_NOTIFICATIONS.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/accessibility";
  @import '../../style/colours';
  @import '../../style/textstyles';
  @import '../../style/fonts';
  @import "../../style/webshared";
</style>
