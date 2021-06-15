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
               :text="$t('more.nhsLogin')"
               :aria-label="$t('more.nhsLogin')"
               :click-func="goToNHSSettings"/>
    <menu-item v-if="showNotifications"
               id="btn_notificationOptions"
               :header-tag="headerTag"
               :text="$t('more.notifications')"
               :aria-label="$t('more.notifications')"
               :click-func="showNotificationsClicked"/>

    <third-party-jump-off-button v-if="showGncrAccountAdmin"
                                 id="btn_gncr_admin"
                                 provider-id="gncr"
                                 :provider-configuration="thirdPartyProvider.gncr.admin" />
  </div>
</template>

<script>
import { INTERSTITIAL_REDIRECTOR_PATH, MORE_NOTIFICATIONS_PATH, MORE_LOGIN_SETTINGS_PATH } from '@/router/paths';
import { REDIRECT_PARAMETER } from '@/router/names';
import MenuItem from '@/components/MenuItem';
import NativeApp from '@/services/native-app';
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
      if (this.$store.getters['loginSettings/biometricSupported']) {
        const biometricType = this.$store.getters['loginSettings/biometricType'];
        return `loginSettings.biometrics.biometricType.${biometricType}`;
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
  methods: {
    goToLoginOptions() {
      redirectTo(this, MORE_LOGIN_SETTINGS_PATH);
    },
    showNotificationsClicked() {
      redirectTo(this, MORE_NOTIFICATIONS_PATH);
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
        if (NativeApp.supportsNativeWebIntegration()) {
          redirectTo(this, `/${INTERSTITIAL_REDIRECTOR_PATH}?${REDIRECT_PARAMETER}=${settingsUrl}`);
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
