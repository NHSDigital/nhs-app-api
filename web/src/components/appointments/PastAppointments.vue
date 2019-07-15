<template>
  <span>
    <h2 :class="!$store.state.device.isNativeApp && $style.desktopWeb">
      {{ $t('appointments.index.past.header') }}</h2>

    <div role="list">
      <appointment v-for="appointment in appointments"
                   :key="appointment.id"
                   :class="$style.past"
                   :appointment="appointment"
                   :cancellation-disabled="true"
                   :show-cancellation-link="false"
                   :telephone-message="$t('appointments.index.past.telephoneMessage')"
                   data-purpose="past-appointments"
                   role="listitem"/>
    </div>
  </span>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';

export default {
  name: 'PastAppointments',
  components: {
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
