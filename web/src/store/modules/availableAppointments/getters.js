export default {
  isBookingAppointmentInProgress(state) {
    return !!state.bookingInProgress;
  },
};
