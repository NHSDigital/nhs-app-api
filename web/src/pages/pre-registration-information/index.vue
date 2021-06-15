<template>
  <nhs-uk-app-layout>
    <div>
      <p class="nhsuk-u-margin-bottom-4">{{ $t('login.useTheNhsAppTo') }}</p>
      <ul>
        <li class="nhsuk-u-margin-bottom-3">{{ $t('login.getYourCovidPass') }}</li>
        <li class="nhsuk-u-margin-bottom-3">{{ $t('login.orderRepeatPrescriptions') }}</li>
        <li class="nhsuk-u-margin-bottom-3">{{ $t('login.bookAndManageAppointments') }}</li>
        <li class="nhsuk-u-margin-bottom-3">{{ $t('login.getHealthInfoAndAdvice') }}</li>
        <li class="nhsuk-u-margin-bottom-3">{{ $t('login.viewYourHealthRecord') }}</li>
        <li class="nhsuk-u-margin-bottom-3">{{ $t('login.manageOrganDonationDecision') }}</li>
        <li>{{ $t('login.viewYourNhsNumber') }}</li>
      </ul>
      <pre-registration-information :should-show-full-content="false"/>
      <p class="nhsuk-u-margin-bottom-4">{{ $t('login.toGetStarted') }}</p>
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
