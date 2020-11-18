<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <menu-item-list data-purpose="settings-menu">
      <menu-item v-if="hasLinkedProfiles"
                 id="'linked-profiles-link'"
                 header-tag="h2"
                 :href="linkedProfilesPath"
                 :text="$t('account.linkedProfiles')"
                 :click-func="navigateToLinkedProfiles"/>
      <menu-item id="'cookies'"
                 header-tag="h2"
                 :href="cookiesPath"
                 :text="$t('account.cookies.cookies')"
                 :click-func="goToUrl"
                 :click-param="cookiesPath"/>
      <settings data-purpose="setting-section"
                :show-notifications="showNotifications"
                :show-biometrics="showBiometrics"/>
    </menu-item-list>

    <template v-if="$store.state.device.isNativeApp">
      <analytics-tracked-tag :text="$t('generic.logOut')"
                             data-purpose="button">
        <generic-button id="signout-button"
                        data-purpose="logout-button"
                        class="nhsuk-button nhsuk-button--secondary"
                        @click.prevent="signout">
          {{ $t('generic.logOut') }}
        </generic-button>
      </analytics-tracked-tag>
    </template>

    <about-us/>

    <p>
      {{ $t('generic.version') }} {{ $store.state.appVersion.webVersion }}
      <span v-show="$store.state.appVersion.nativeVersion">
        ({{ $store.state.appVersion.nativeVersion }})
      </span>
    </p>
    <p v-if="showCEMark">
      <ce-mark-icon/>
    </p>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AboutUs from '@/components/account/AboutUs';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import CeMarkIcon from '@/components/icons/CeMarkIcon';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import NativeCallbacks from '@/services/native-app';
import Settings from '@/components/account/Settings';
import sjrIf from '@/lib/sjrIf';
import canVersionHandleBiometricsWeb from '@/lib/biometrics/canVersionHandleBiometricsWeb';
import GenericButton from '@/components/widgets/GenericButton';
import { ACCOUNT_COOKIES_PATH, LINKED_PROFILES_PATH } from '@/router/paths';
import { isTruthy } from '@/lib/utils';

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
      cookiesPath: ACCOUNT_COOKIES_PATH,
      linkedProfilesPath: LINKED_PROFILES_PATH,
    };
  },
  computed: {
    showBiometrics() {
      if (!canVersionHandleBiometricsWeb(this)) {
        return this.nativeLoginOptionsMethodExists &&
          this.$store.state.device.isNativeApp;
      }
      return this.$store.state.device.isNativeApp;
    },
    showNotifications() {
      return sjrIf({ $store: this.$store, journey: 'notifications' }) &&
        this.$store.state.device.isNativeApp;
    },
    hasLinkedProfiles() {
      return this.$store.getters['linkedAccounts/hasLinkedAccounts'];
    },
    showCEMark() {
      return isTruthy(this.$store.$env.CE_MARK_ENABLED);
    },
  },
  mounted() {
    if (!canVersionHandleBiometricsWeb(this)) {
      this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
    }
  },
  methods: {
    signout() {
      this.$store.dispatch('auth/logout');
    },
    goToLoginOptions() {
      NativeCallbacks.goToLoginOptions();
    },
    navigateToLinkedProfiles() {
      this.$store.dispatch('navigation/setRouteCrumb', 'settingsCrumb');
      this.goToUrl(this.linkedProfilesPath);
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
