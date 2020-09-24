<template>
  <nhs-uk-app-layout>
    <div>
      <pre-registration-information :should-show-header="false"/>
      <login-button button-text="generic.continue"/>
    </div>
  </nhs-uk-app-layout>
</template>

<script>
import PreRegistrationInformation from '@/components/PreRegistrationInformation';
import LoginButton from '@/components/widgets/LoginButton';
import NativeCallbacks from '@/services/native-app';
import NhsUkAppLayout from '@/layouts/nhsuk-layout';

export default {
  name: 'PreRegistrationInformationPage',
  components: {
    PreRegistrationInformation,
    LoginButton,
    NhsUkAppLayout,
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
