<template>
  <div v-if="showBiometricBanner">
    <message-dialog :extra-classes="[$style['flash-message']]"
                    message-id="success-dialog"
                    message-type="message"
                    :icon-text="$t('biometricBanner.header')">
      <message-text>
        {{ $t('biometricBanner.message.text') }}
      </message-text>
      <message-text>
        <analytics-tracked-tag :text="$t('biometricBanner.message.settingsButton')">
          <generic-button
            id="btn_goToSettings"
            :button-classes="['nhsuk-button',
                              'nhsuk-button--secondary',
                              $style['nhsuk-button-full-width']]"
            tabindex="0"
            @click="goToLoginOptions">
            {{ $t('biometricBanner.message.settingsButton') }}
          </generic-button>
        </analytics-tracked-tag>
        <p :class="['nhsuk-u-margin-bottom-0', $style['center']]">
          <analytics-tracked-tag id="btn_biometricBannerDismiss"
                                 :text="$t('biometricBanner.message.dismissLink')"
                                 tag="a"
                                 :click-func="dismissBiometricsBannerClicked">
            {{ $t('biometricBanner.message.dismissLink') }}
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
import canVersionHandleBiometricsWeb from '@/lib/biometrics/canVersionHandleBiometricsWeb';
import { LOGIN_SETTINGS_PATH } from '@/router/paths';
import NativeCallbacks from '@/services/native-app';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'BiometricBanner',
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      nativeLoginOptionsMethodExists: true,
    };
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
  mounted() {
    this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
  },
  methods: {
    dismissBiometricsBannerClicked() {
      this.$store.dispatch('biometricBanner/dismiss');
      this.$store.dispatch('biometricBanner/sync');
      NativeCallbacks.resetPageFocus();
    },
    goToLoginOptions() {
      if (canVersionHandleBiometricsWeb(this)) {
        redirectTo(this, LOGIN_SETTINGS_PATH);
      } else {
        this.configureWebContext(this.$route.meta.helpUrl);
        NativeCallbacks.goToLoginOptions();
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
  .nhsuk-button-full-width {
    width: 100%;
  }
  a {
    display: inline;
    cursor: pointer;
  }
  .center {
    text-align: center;
  }
  .msgText {
    padding: 1em 1em 1em 1em;
    margin-top:0;
    background-color: #ffffff;
  }
  .msgText a {
    text-align: center;
  }
  .flash-message {
    margin-top: 1.125em;
    margin-bottom: 1.25em;
  }
</style>
