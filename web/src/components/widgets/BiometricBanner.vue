<template>
  <div v-if="showBiometricBanner" :class="$style.container">
    <message-dialog :extra-classes="[$style['flash-message']]"
                    message-id="success-dialog"
                    message-type="message"
                    :icon-text="$t('home.loginOptions')">
      <message-text>
        {{ $t('home.ifYourDeviceSupports') }}
      </message-text>
      <message-text>
        <analytics-tracked-tag :text="$t('home.openSettings')" tabindex="">
          <generic-button
            id="btn_goToSettings"
            :button-classes="['nhsuk-button',
                              'nhsuk-button--secondary',
                              $style['nhsuk-button-full-width']]"
            tabindex="0"
            @click="goToLoginOptions">
            {{ $t(`home.${getBiometricLinkText}`) }}
          </generic-button>
        </analytics-tracked-tag>
        <p :class="['nhsuk-u-margin-bottom-0', $style['center']]">
          <analytics-tracked-tag id="btn_biometricBannerDismiss"
                                 :text="$t('home.dismiss')"
                                 tag="a"
                                 :click-func="dismissBiometricsBannerClicked">
            {{ $t('home.dismiss') }}
          </analytics-tracked-tag>
        </p>
      </message-text>
    </message-dialog>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import {
  MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH,
  MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import biometricTypes from '@/lib/biometrics/biometricTypes';

export default {
  name: 'BiometricBanner',
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    MessageDialog,
    MessageText,
  },
  computed: {
    formData() {
      return {
        biometricBanner: {
          dismissed: true,
        },
      };
    },
    showBiometricBanner() {
      return this.$store.state.device.isNativeApp && !this.$store.state.biometricBanner.dismissed;
    },
    getBiometricLinkText() {
      const biometricSupported = this.$store.getters['loginSettings/biometricSupported'];
      const biometricType = this.$store.getters['loginSettings/biometricType'];

      if (biometricSupported) {
        if (biometricType === biometricTypes.Fingerprint) {
          return 'setupFingerPrint';
        }
        if (biometricType === biometricTypes.FaceID) {
          return 'setupFaceId';
        }
        if (biometricType === biometricTypes.TouchID) {
          return 'setupTouchId';
        }
        return 'setupLoginOptions';
      }
      return 'openLoginSettings';
    },
  },
  created() {
    this.$store.dispatch('biometricBanner/sync');
  },
  methods: {
    dismissBiometricsBannerClicked() {
      this.$store.dispatch('biometricBanner/dismiss');
      this.$store.dispatch('biometricBanner/sync');
    },
    goToLoginOptions() {
      const biometricSupported = this.$store.getters['loginSettings/biometricSupported'];
      const biometricType = this.$store.getters['loginSettings/biometricType'];

      let redirectUrl = MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH;

      if (biometricSupported) {
        if (biometricType === biometricTypes.Fingerprint) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH;
        } else if (biometricType === biometricTypes.FaceID) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH;
        } else if (biometricType === biometricTypes.TouchID) {
          redirectUrl = MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH;
        }
      }

      redirectTo(this, redirectUrl);
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/biometric-banner";
</style>
