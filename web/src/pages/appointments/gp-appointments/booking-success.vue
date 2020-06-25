<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="isProxying">
          <p>
            {{ $tc('appointments.bookingSuccess.proxyMessage', null, { name }) }}
          </p>
          <switch-profile-button id="switchProfileButton"/>
        </div>
        <div v-else>
          <CardGroup class="nhsuk-grid-row">
            <CardGroupItem class="nhsuk-grid-column-one-half">
              <Card>
                <appointment-slot v-if="slot"
                                  id="appointmentDetails"
                                  :appointment="slot"
                                  :show-cancellation-link="false"
                                  data-purpose="appointment-info"
                                  date-time-header="h2"/>
              </Card>
            </CardGroupItem>
          </CardGroup>
          <desktopGenericBackLink
            id="genericBackLink"
            :path="backPath"
            :button-text="'appointments.bookingSuccess.back'"
            @clickAndPrevent="backButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AppointmentSlot from '@/components/appointments/Appointment';
import Card from '@/components/widgets/card/Card';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import get from 'lodash/fp/get';
import { APPOINTMENTS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'BookingSuccess',
  layout: 'nhsuk-layout',
  components: {
    AppointmentSlot,
    Card,
    CardGroup,
    CardGroupItem,
    DesktopGenericBackLink,
    SwitchProfileButton,
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      name: get('$store.state.linkedAccounts.actingAsUser.fullName', this),
      backPath: APPOINTMENTS.path,
      slot: this.$store.state.availableAppointments.selectedSlot,
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
    this.$store.dispatch('availableAppointments/completeBookingJourney');
  },
  beforeDestroy() {
    this.$store.dispatch('availableAppointments/deselect');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
