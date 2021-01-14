<template>
  <div :class="[$style.appointmentTime, 'nhsuk-expander-group']">
    <collapsible-details v-for="daySlots in availableSlotsWithAppointments"
                         :key="formatDate(daySlots[0])"
                         class="nhsuk-expander"
                         data-purpose="appointment-day">
      <template slot="header">
        <span data-purpose="appointment-day-heading">
          {{ formatDate(daySlots[0]) }}
        </span>
        <p class="nhsuk-u-margin-bottom-0">
          <span id="appointmentCount" :class="$style['subHeader']">
            {{ availableAppointmentCount(daySlots[1].length) }}
          </span>
        </p>
      </template>
      <ul :class="[$style['selector-list'], 'nhsuk-u-padding-left-0']">
        <time-slot v-for="slot in daySlots[1]"
                   :key="slot.ref"
                   :ref="slot.ref"
                   :time-slot="slot"
                   @click.native="select(slot.ref)"
                   @select="select($event)" />
      </ul>
    </collapsible-details>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import TimeSlot from '@/components/appointments/booking/TimeSlot';
import DateProvider from '@/services/DateProvider';
import { APPOINTMENT_CONFIRMATIONS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';

export default {
  name: 'SlotList',
  components: {
    TimeSlot,
    CollapsibleDetails,
  },
  props: {
    availableSlots: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    availableSlotsWithAppointments() {
      return this.availableSlots.filter(this.hasAppointments);
    },
  },
  methods: {
    formatDate: dateTime => DateProvider.create(dateTime).format('dddd D MMMM YYYY'),
    select(ref) {
      this.$refs[ref][0].select();
      redirectTo(this, APPOINTMENT_CONFIRMATIONS_PATH);
    },
    hasAppointments(daySlots) {
      return daySlots[1].length > 0;
    },
    availableAppointmentCount(slotCount) {
      return this.$tc(
        'appointments.book.numberOfAppointmentsAvailable',
        slotCount, { appointmentsCount: slotCount },
      );
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/slot-list";
</style>
