<template>
  <span>
    <h2 :class="!$store.state.device.isNativeApp && $style.desktopWeb">
      {{ $t('appointments.index.past.header') }}</h2>

    <div role="list">
      <appointment v-for="appointment in appointments"
                   :class="$style.past"
                   :key="appointment.id"
                   :appointment="appointment"
                   :cancellation-disabled="true"
                   :show-cancellation-link="false"
                   data-purpose="past-appointments"
                   role="listitem"/>
    </div>
  </span>
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
  },
};
</script>

<style module lang="scss" scoped>

.past {
  display: table !important;
}

h2 {
 &.desktopWeb {
  margin-top: 1em;
 }
}

</style>
