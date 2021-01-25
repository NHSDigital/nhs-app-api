<template>
  <span>
    <h2 class="nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-0 break">
      {{ $t('appointments.past.pastAppointments') }}
    </h2>
    <CardGroup role="list" class="nhsuk-grid-row">
      <CardGroupItem v-for="appointment in appointments"
                     :key="appointment.id" class="nhsuk-grid-column-one-half">
        <Card class="nhsuk-u-margin-bottom-5">
          <appointment :appointment="appointment"
                       :cancellation-disabled="true"
                       :show-cancellation-link="false"
                       :telephone-message="$t('appointments.past.thePhoneNumberYouGave')"
                       data-purpose="past-appointments"
                       role="listitem"/>
        </Card>
      </CardGroupItem>
    </CardGroup>
  </span>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  name: 'PastAppointments',
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
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/break";
</style>
