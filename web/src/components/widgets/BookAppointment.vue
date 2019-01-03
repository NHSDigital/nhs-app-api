<template>
  <span :class="$style.appointmentBooking">
    <no-js-form :action="guidancePath" :value="formData">
      <button v-if="showBookAppointmentButton"
              id="book-appointments-header-button"
              :class="$style.bookingButton"
              @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.index.bookButtonText') }}
      </button>
    </no-js-form>
  </span>
</template>
<script>
/* eslint-disable no-unused-vars */
import { APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';
import NoJsForm from '@/components/no-js/NoJsForm';

export default {
  components: {
    NoJsForm,
  },
  computed: {
    guidancePath() {
      return APPOINTMENT_BOOKING_GUIDANCE.path;
    },
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
    showBookAppointmentButton() {
      return (
        this.$store.state.myAppointments.hasLoaded
      );
    },
  },
  methods: {
    onBookButtonClicked() {
      this.$store.app.$analytics.trackButtonClick(this.guidancePath, true);
      this.$router.push(this.guidancePath);
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import '../../style/colours';
 @import '../../style/screensizes';
 @import '../../style/textstyles';
 @import "../../style/fonts";

 .appointmentBooking {
  display: block;
  margin: 0 auto;
  max-width: 960px;
  padding: 1em 16px 16px;

  .bookingButton {
   @include button;
   box-sizing: border-box;
   padding: 0.625em;
   background-color: $nhs_blue;
   border: none;
   border-radius: 0.125em;
   outline: none;
   transition: all ease 0.5s;
   cursor: pointer;
   width: auto;
   min-width: 16.875em;
   padding-left: 2em;
   padding-right: 2em;
   max-width: 960px;
   display: block;
  }

  .bookingButton:focus {
    outline-color: $focus_highlight;
    box-shadow: 0 0 0 4px $focus_highlight;
  }
 }
</style>
