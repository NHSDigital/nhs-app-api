<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <menu-item-list data-purpose="cookie-menu">
      <menu-item id="'cookies'"
                 header-tag="h2"
                 :href="cookiesPath"
                 :text="$t('myAccount.cookiesLink')"
                 :click-func="goToUrl"
                 :click-param="cookiesPath"/>
    </menu-item-list>

    <template v-if="$store.state.device.isNativeApp">
      <settings v-if="(showBiometrics || showNotifications)"
                data-purpose="setting-section"
                :show-notifications="showNotifications"
                :show-biometrics="showBiometrics"/>

      <analytics-tracked-tag :text="$t('signOutButton.signOut')"
                             data-purpose="button"
                             :tabindex="-1">
        <form action="account/signout" method="post">
          <generic-button id="signout-button"
                          data-purpose="logout-button"
                          class="nhsuk-button nhsuk-button--secondary"
                          @click.prevent="signout">
            {{ $t('signOutButton.signOut') }}
          </generic-button>
        </form>
      </analytics-tracked-tag>
    </template>

    <about-us/>

    <p>
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </p>
    <p v-if="this.$store.app.$env.CE_MARK_ENABLED">
      <ce-mark-icon/>
    </p>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { ACCOUNT_COOKIES } from '@/lib/routes';
import AboutUs from '@/components/account/AboutUs';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CeMarkIcon from '@/components/icons/CeMarkIcon';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import NativeCallbacks from '@/services/native-app';
import Settings from '@/components/account/Settings';
import sjrIf from '@/lib/sjrIf';
import GenericButton from '@/components/widgets/GenericButton';

export default {
  layout: 'nhsuk-layout',
  components: {
    AboutUs,
    CeMarkIcon,
    MenuItem,
    MenuItemList,
    Settings,
    GenericButton,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      nativeLoginOptionsMethodExists: true,
      cookiesPath: ACCOUNT_COOKIES.path,
    };
  },
  computed: {
    showBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.nativeLoginOptionsMethodExists &&
        this.$store.state.device.isNativeApp;
    },
    showNotifications() {
      return sjrIf({ $store: this.$store, journey: 'notifications' }) &&
        this.$store.state.device.isNativeApp;
    },
  },
  mounted() {
    this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
    if (this.$store.state.device.isNativeApp) {
      this.$store.dispatch('header/updateHeaderText', this.$t('pageHeaders.settings'));
    }
  },
  methods: {
    signout() {
      this.$store.dispatch('auth/logout');
    },
    goToLoginOptions() {
      NativeCallbacks.goToLoginOptions();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/accessibility";
@import "../../style/listmenu";
@import "../../style/colours";
@import "../../style/webshared";

</style>
