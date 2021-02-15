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
               :text="$t('account.nhsLogin')"
               :aria-label="$t('account.nhsLogin')"
               :href="settingsUrl"/>
    <menu-item v-if="showNotifications"
               id="btn_notificationOptions"
               :header-tag="headerTag"
               :text="$t('account.notifications')"
               :aria-label="$t('account.notifications')"
               :click-func="showNotificationsClicked"/>

    <third-party-jump-off-button v-if="showGncrAccountAdmin"
                                 id="btn_gncr_admin"
                                 provider-id="gncr"
                                 :provider-configuration="thirdPartyProvider.gncr.admin" />
  </div>
</template>

<script>
import { ACCOUNT_NOTIFICATIONS_PATH, LOGIN_SETTINGS_PATH } from '@/router/paths';
import MenuItem from '@/components/MenuItem';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'Settings',
  components: {
    MenuItem,
    ThirdPartyJumpOffButton,
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
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      hasGncrAccountAdmin: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'gncr',
          serviceType: 'accountAdmin',
        },
      }),
    };
  },
  computed: {
    getBiometricLinkText() {
      if (this.$store.getters['loginSettings/getDeviceBiometricNameString'] !== undefined) {
        return this.$store.getters['loginSettings/getDeviceBiometricNameString'];
      }

      return 'loginSettings.biometrics.noBiometricType.settingsLinkText';
    },
    showGncrAccountAdmin() {
      return this.hasGncrAccountAdmin && this.isProofLevel9;
    },
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
  },
  mounted() {
    this.goToNHSSettings();
  },
  methods: {
    goToLoginOptions() {
      redirectTo(this, LOGIN_SETTINGS_PATH);
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
      this.settingsUrl = `${this.cidSettingsUrl}?asserted_login_identity=${token}`;
    },
  },
};
</script>
