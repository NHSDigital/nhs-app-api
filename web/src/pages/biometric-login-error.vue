<template>
  <nhsuk-layout>
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
  </nhsuk-layout>
</template>

<script>
import NhsukLayout from '@/layouts/nhsuk-layout';
import NativeCallbacks from '@/services/native-app';

export default {
  name: 'BiometricLoginErrorPage',
  components: {
    NhsukLayout,
  },
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
