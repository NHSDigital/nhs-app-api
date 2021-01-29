<template>
  <nhs-uk-app-layout>
    <div>
      <pre-registration-information :should-show-header="false"/>
      <generic-button id="login-button"
                      class="nhsuk-u-margin-bottom-3"
                      :button-classes="['nhsuk-button']"
                      type="submit"
                      data-id="login-button"
                      @click="trackLogin">
        {{ $t("generic.continue") }}
      </generic-button>
    </div>
  </nhs-uk-app-layout>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import NativeApp from '@/services/native-app';
import NhsUkAppLayout from '@/layouts/nhsuk-layout';
import PreRegistrationInformation from '@/components/PreRegistrationInformation';
import { BEGINLOGIN_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'PreRegistrationInformationPage',
  components: {
    GenericButton,
    NhsUkAppLayout,
    PreRegistrationInformation,
  },
  data() {
    return {
      isButtonDisabled: false,
    };
  },
  mounted() {
    NativeApp.showHeaderSlim();
    NativeApp.hideWhiteScreen();
  },
  methods: {
    async trackLogin() {
      if (!this.isButtonDisabled) {
        this.isButtonDisabled = true;

        this.$store.dispatch('analytics/satelliteTrack', 'login');
        await this.$store.dispatch('preRegistrationInformation/continue');
        redirectTo(this, BEGINLOGIN_PATH, this.$route.query);
      }
    },
  },
};
</script>
