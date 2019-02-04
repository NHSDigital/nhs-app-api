<template>
  <span>
    <h2 :class="[isDesktopWeb ? $style.desktopWeb : undefined]">
      {{ $t('appointments.index.past.header') }}</h2>

    <appointment v-for="appointment in appointments"
                 :class="[$style.past]"
                 :key="appointment.id"
                 :appointment="appointment"
                 :cancellation-disabled="true"
                 :show-cancellation-link="false"
                 data-purpose="past-appointments" />
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
  data() {
    return {
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
    };
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
