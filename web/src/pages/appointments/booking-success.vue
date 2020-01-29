<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="isProxying">
          <p>
            {{ $tc('appointments.bookingSuccess.proxyMessage', null, { name }) }}
          </p>
          <switch-profile-button />
        </div>
        <div v-else>
          <desktopGenericBackLink
            :path="backPath"
            :button-text="'appointments.bookingSuccess.back'"
            @clickAndPrevent="backButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import get from 'lodash/fp/get';
import { APPOINTMENTS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'BookingSuccess',
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    SwitchProfileButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      name: get('$store.state.linkedAccounts.actingAsUser.name', this),
      backPath: APPOINTMENTS.path,
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
    this.$store.dispatch('availableAppointments/completeBookingJourney');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
