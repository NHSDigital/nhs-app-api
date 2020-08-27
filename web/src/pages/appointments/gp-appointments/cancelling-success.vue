<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="isProxying">
        <p>
          {{ $tc('appointments.cancel.youHaveCancelledNamesAppointment', null, { name }) }}
        </p>
        <switch-profile-button />
      </div>
      <div v-else>
        <desktopGenericBackLink
          :path="backPath"
          :button-text="'appointments.cancel.goToYourAppointments'"
          @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';

import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

import { APPOINTMENTS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'GpAppointmentsCancellingSuccessPage',
  components: {
    DesktopGenericBackLink,
    SwitchProfileButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      name: get('$store.state.linkedAccounts.actingAsUser.fullName', this),
      backPath: APPOINTMENTS_PATH,
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
    this.$store.dispatch('myAppointments/completeCancellingJourney');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
