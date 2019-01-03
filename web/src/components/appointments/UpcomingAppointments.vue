<template>
  <div>
    <h2>{{ $t('appointments.index.upcoming.header') }}</h2>

    <appointment v-for="appointment in appointments"
                 :class="[$style.upcoming]"
                 :key="appointment.id"
                 :appointment="appointment"
                 :cancellation-disabled="cancellationDisabled"
                 data-purpose="appointments" />
  </div>
</template>

<script>
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';
import Appointment from '@/components/appointments/Appointment';

export default {
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
