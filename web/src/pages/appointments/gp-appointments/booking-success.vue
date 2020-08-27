<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="isProxying">
          <p>
            {{ $tc('appointments.book.youHaveBookedFor', null, { name }) }}
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
                                  :show-add-to-calendar-link="true"
                                  data-purpose="appointment-info"
                                  date-time-header="h2"/>
              </Card>
            </CardGroupItem>
          </CardGroup>
          <desktopGenericBackLink
            id="genericBackLink"
            :path="backPath"
            :button-text="'appointments.book.goToYourAppointments'"
            @clickAndPrevent="backButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AppointmentSlot from '@/components/appointments/Appointment';
import Card from '@/components/widgets/card/Card';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import { APPOINTMENTS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'GpAppointmentsBookingSuccessPage',
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
      backPath: APPOINTMENTS_PATH,
      slot: this.$store.state.availableAppointments.selectedSlot,
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
