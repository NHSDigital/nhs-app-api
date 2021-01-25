<template>
  <div>
    <h2 class="nhsuk-u-margin-bottom-0 break">
      {{ $t('appointments.upcoming.upcomingAppointments') }}
    </h2>
    <CardGroup role="list" class="nhsuk-grid-row">
      <CardGroupItem v-for="appointment in appointments"
                     :key="appointment.id" class="nhsuk-grid-column-one-half">
        <Card class="nhsuk-u-margin-bottom-5">
          <appointment :appointment="appointment"
                       :cancellation-disabled="cancellationDisabled"
                       :telephone-message="$t('appointments.upcoming.weWillCallYouOn')"
                       data-purpose="upcoming-appointments"
                       class="nhsuk-u-margin-bottom-3"
                       role="listitem"/>
        </Card>
      </CardGroupItem>
    </CardGroup>
  </div>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';
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
};
</script>

<style lang="scss">
  @import "@/style/custom/break";
</style>
