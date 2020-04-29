<template>
  <div>
    <pre-registration-information :should-show-header="shouldShowPreRegistrationHeader"/>
    <other-services/>
    <login-button button-text="preRegistrationInformation.buttonText"/>
  </div>
</template>

<script>
import PreRegistrationInformation from '@/components/PreRegistrationInformation';
import OtherServices from '@/components/OtherServices';
import LoginButton from '@/components/widgets/LoginButton';
import NativeCallbacks from '@/services/native-app';

export default {
  layout: 'nhsuk-layout',
  components: {
    PreRegistrationInformation,
    OtherServices,
    LoginButton,
  },
  data() {
    return {
      shouldShowPreRegistrationHeader: false,
    };
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
