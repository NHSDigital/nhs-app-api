<template>
  <main :class="$style.main">
    <success-dialog v-if="justBookedAnAppointment">
      <p>
        {{ $t('appointments.index.successText') }}
      </p>
    </success-dialog>
    <div :class="$style.info">
      <h3>{{ $t('appointments.index.empty.header') }}</h3>
      <p>{{ $t('appointments.index.empty.text1') }}</p>
      <p>{{ $t('appointments.index.empty.text2') }} </p>
    </div>
    <floating-button-bottom @on-click="onBookButtonClicked">
      {{ $t('appointments.index.bookButtonText') }}
    </floating-button-bottom>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import SuccessDialog from '@/components/SuccessDialog';
import FloatingButtonBottom from '../../components/FloatingButtonBottom';

export default {
  middleware: ['auth', 'meta'],
  components: {
    SuccessDialog,
    FloatingButtonBottom,
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
  beforeDestroy() {
    this.$store.dispatch('appointmentSlots/reset');
  },
  methods: {
    onBookButtonClicked() {
      this.$router.push('/appointments/booking');
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";
  @import "../../style/textstyles";
  @import "../../style/colours";

  .main {
    @include space(padding, all, $three);

    .info {
      p {
        @include default_text;
        font-size: 12pt;
        display: block;
        margin-bottom: 16px;
      }
      h3 {
        @include h5;
        font-size: 13pt;
        color: $dark_grey;
        margin-bottom: 16px;
      }
    }
  }

</style>
