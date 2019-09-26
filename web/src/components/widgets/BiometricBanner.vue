<template>
  <div v-if="showBiometricBanner">
    <message-dialog :extra-classes="[$style['flash-message']]"
                    message-id="success-dialog"
                    message-type="message"
                    :icon-text="$t('biometricBanner.header')">
      <message-text>
        {{ $t('biometricBanner.message.text') }}
      </message-text>
      <div :class="[$style.msgText,
                    !$store.state.device.isNativeApp && $style.desktopWeb]">
        <analytics-tracked-tag :text="$t('biometricBanner.message.settingsButton')">
          <generic-button
            :button-classes="['button' , 'grey']"
            tabindex="0"
            @click="goToLoginOptions">
            {{ $t('biometricBanner.message.settingsButton') }}
          </generic-button>
        </analytics-tracked-tag>
        <analytics-tracked-tag id="btn_biometricBannerDismiss"
                               :text="$t('biometricBanner.message.dismissLink')"
                               tag="a"
                               :click-func="dismissBiometricsBannerClicked">
          {{ $t('biometricBanner.message.dismissLink') }}
        </analytics-tracked-tag>
      </div>
    </message-dialog>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import GenericButton from '@/components/widgets/GenericButton';
import NativeCallbacks from '@/services/native-app';
import { setCookie } from '@/lib/cookie-manager';
import { findByName } from '../../lib/routes';
import moment from 'moment';

export default {
  name: 'BiometricBanner',
  components: {
    AnalyticsTrackedTag,
    MessageDialog,
    MessageText,
    GenericButton,
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
      const hideBiometricBannerCookie = this.$store.app.$cookies.get('HideBiometricBanner');
      return (this.$store.state.device.source === 'android' || this.$store.state.device.source === 'ios') &&
        (hideBiometricBannerCookie === undefined || hideBiometricBannerCookie === false);
    },
  },
  mounted() {
    this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
  },
  methods: {
    goToLoginOptions() {
      this.setHelpUrl(findByName('Login').helpUrl);
      NativeCallbacks.goToLoginOptions();
    },
    dismissBiometricsBannerClicked() {
      let hideBiometricBannerCookie = this.$store.app.$cookies.get('HideBiometricBanner');

      hideBiometricBannerCookie = {
        ...hideBiometricBannerCookie,
        Dismissed: true,
      };

      setCookie({
        cookies: this.$store.app.$cookies,
        key: 'HideBiometricBanner',
        value: hideBiometricBannerCookie.Dismissed,
        options: {
          maxAge: moment.duration(5, 'y').asSeconds(),
          secure: this.$store.app.$env.SECURE_COOKIES,
        },
      });
      this.$store.dispatch('biometricBanner/refreshPage');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';
  @import "../../style/textstyles";
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
