<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div class="nhsuk-u-padding-top-4">
        <p data-sid="biometricLoginErrorParagraphOne">
          {{ $t('loginBiometricError.paragraph1') }}
        </p>
        <p data-sid="biometricLoginErrorParagraphTwo">
          {{ $t('loginBiometricError.paragraph2') }}
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import NativeCallbacks from '@/services/native-app';

export default {
  layout: 'nhsuk-layout',
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.showHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
    } else {
      window.scrollTo(0, 0);
    }
  },
};
</script>
