<template>
  <div :class="$style.appointments">
    <div class="panel-title">
      <h2>{{ $t('appointments.index.upcoming.header') }}</h2>
    </div>
    <div data-purpose="appointments">
      <appointment v-for="appointment in appointments"
                   :key="appointment.id" :appointment="appointment" />
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
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
  },
};
</script>

<style module lang="scss">
  @import "../../style/textstyles";

  .appointments {
    margin-bottom: 80px;
    h2 {
      @include h2Blue;
      margin-bottom: 0.5em
    }
  }

</style>
