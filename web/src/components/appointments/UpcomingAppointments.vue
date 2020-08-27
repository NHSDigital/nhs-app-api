<template>
  <div>
    <h2 class="nhsuk-u-margin-bottom-0">{{ $t('appointments.upcoming.upcomingAppointments') }}</h2>
    <CardGroup v-for="(chunk, index) in chunked" :key="index" role="list" class="nhsuk-grid-row">
      <CardGroupItem v-for="appointment in chunk"
                     :key="appointment.id" class="nhsuk-grid-column-one-half">
        <Card>
          <appointment :appointment="appointment"
                       :cancellation-disabled="cancellationDisabled"
                       :telephone-message="$t('appointments.upcoming.weWillCallYouOn')"
                       data-purpose="upcoming-appointments"
                       role="listitem"/>
        </Card>
      </CardGroupItem>
    </CardGroup>
  </div>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';
import chunk from 'lodash/fp/chunk';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  name: 'UpcomingAppointment',
  components: {
    Card,
    CardGroupItem,
    CardGroup,
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
  computed: {
    chunked() {
      return chunk(2)(this.appointments);
    },
  },
};
</script>
