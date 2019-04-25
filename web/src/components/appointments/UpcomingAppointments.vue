<template>
  <span>
    <h2>{{ $t('appointments.index.upcoming.header') }}</h2>

    <div role="list">
      <appointment v-for="appointment in appointments"
                   :key="appointment.id"
                   :class="$style.upcoming"
                   :appointment="appointment"
                   :cancellation-disabled="cancellationDisabled"
                   data-purpose="upcoming-appointments"
                   role="listitem"/>
    </div>
  </span>
</template>

<script>
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';
import Appointment from '@/components/appointments/Appointment';

export default {
  name: 'UpcomingAppointment',
  components: {
    ClinicianIcon,
    LocationIcon,
    Appointment,
  },
  props: {
    appointments: {
      type: Array,
      default: () => [],
    },
    cancellationDisabled: {
      default: false,
      type: Boolean,
    },
  },
};
</script>

<style module lang="scss" scoped>

.upcoming {
  display: table !important;
}

</style>
