<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <menu-item-list data-purpose="account-and-settings-menu">
      <accountAndSettings data-purpose="setting-section"
                          :show-notifications="showNotifications"
                          :show-biometrics="showBiometrics"/>
      <menu-item id="'cookies'"
                 header-tag="h2"
                 :href="legalAndCookiesPath"
                 :text="$t('accountAndSettings.legalAndCookies')"
                 :aria-label="$t('accountAndSettings.legalAndCookies')"
                 :click-func="goToUrl"
                 :click-param="legalAndCookiesPath"/>
    </menu-item-list>

    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            data-purpose="back-link"
                            :path="backPath"
                            :button-text="'generic.back'"
                            @clickAndPrevent="onBackButtonClicked"/>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import NativeCallbacks from '@/services/native-app';
import AccountAndSettings from '@/components/more/account-and-settings/AccountAndSettings';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import sjrIf from '@/lib/sjrIf';
import { MORE_PATH, MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
    AccountAndSettings,
    DesktopGenericBackLink,
  },
  data() {
    return {
      legalAndCookiesPath: MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_PATH,
      backPath: MORE_PATH,
    };
  },
  computed: {
    showBiometrics() {
      return this.$store.state.device.isNativeApp;
    },
    showNotifications() {
      return sjrIf({ $store: this.$store, journey: 'notifications' }) &&
        this.$store.state.device.isNativeApp;
    },
  },
  methods: {
    goToLoginOptions() {
      NativeCallbacks.goToLoginOptions();
    },
    onBackButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
