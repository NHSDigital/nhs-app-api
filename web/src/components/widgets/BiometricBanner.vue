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
        <analytics-tracked-tag :text="$t('home.openSettings')">
          <generic-button
            id="btn_goToSettings"
            :button-classes="['nhsuk-button',
                              'nhsuk-button--secondary',
                              $style['nhsuk-button-full-width']]"
            tabindex="0"
            @click="goToLoginOptions">
            {{ $t('home.openSettings') }}
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
import { LOGIN_SETTINGS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

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
      redirectTo(this, LOGIN_SETTINGS_PATH);
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/biometric-banner";
</style>
