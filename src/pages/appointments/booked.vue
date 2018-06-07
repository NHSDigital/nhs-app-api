<template>
  <main :class="$style.main">
    <success-dialog v-if="justBookedAnAppointment">
      <p>
        {{ $t('appointments.booked.successText') }}
      </p>
    </success-dialog>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import SuccessDialog from '@/components/SuccessDialog';

export default {
  middleware: ['auth', 'meta'],
  components: {
    SuccessDialog,
  },
  data() {
    return {
      justBookedAnAppointment: false,
    };
  },
  mounted() {
    this.justBookedAnAppointment = this.$store.state.appointment.justBookedAnAppointment;
    this.$store.dispatch('appointment/resetJustBooked');
  },
};
</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/spacings";

  .main {
    @include space(padding, all, $three);
  }

</style>
